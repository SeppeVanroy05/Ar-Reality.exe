import 'dart:async';
import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter_embed_unity/flutter_embed_unity.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:reality_exe/models/glitch.dart';
import 'package:reality_exe/services/ar_bridge.dart';

class UnityMissionPage extends StatefulWidget {
  final Glitch glitch;
  const UnityMissionPage({super.key, required this.glitch});
  @override
  State<UnityMissionPage> createState() => _UnityMissionPageState();
}

class _UnityMissionPageState extends State<UnityMissionPage>
    with WidgetsBindingObserver {
  late ArBridge _bridge;
  Timer? _handshake;
  Timer? _completion;
  bool _cameraAllowed = false;
  bool _checkingPermission = true;
  bool _started = false;
  String? _error;
  int _attempts = 0;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);
    _bridge = ArBridge(widget.glitch);
    _requestCamera();
  }

  Future<void> _requestCamera() async {
    setState(() {
      _checkingPermission = true;
      _error = null;
    });
    try {
      final status = await Permission.camera.request();
      if (!mounted) return;
      setState(() {
        _cameraAllowed = status.isGranted;
        _checkingPermission = false;
        if (!_cameraAllowed) {
          _error =
              'Cameratoegang is nodig om je kaart te herkennen. '
              'Sta de camera toe in de instellingen en probeer opnieuw.';
        }
      });
      if (_cameraAllowed) _startHandshake();
    } catch (_) {
      if (mounted) {
        setState(() {
          _checkingPermission = false;
          _error = 'De camera kon niet worden geopend. Probeer opnieuw.';
        });
      }
    }
  }

  void _startHandshake() {
    _handshake?.cancel();
    _attempts = 0;
    // Ping werkt ook wanneer Unity al geladen is van een vorige missie.
    resumeUnity();
    _handshake = Timer.periodic(const Duration(seconds: 1), (_) {
      if (!mounted || _started) return;
      if (++_attempts > 45) {
        _handshake?.cancel();
        setState(
          () => _error =
              'Unity reageert niet. Controleer de Unity-export en Vuforia-instellingen.',
        );
        return;
      }
      sendToUnity('RealityBridge', 'Ping', '');
    });
  }

  void _onMessage(String message) {
    if (!mounted || _error != null) return;
    try {
      final data = jsonDecode(message);
      if (data is! Map<String, dynamic>) return;
      if (data['type'] == 'error') {
        _handshake?.cancel();
        setState(
          () => _error = data['message'] is String
              ? data['message'] as String
              : 'Unity kon niet starten.',
        );
        return;
      }
      if (data['type'] == 'ready' && data['version'] == 1 && !_started) {
        _handshake?.cancel();
        sendToUnity('RealityBridge', 'BeginMission', _bridge.startMessage);
        setState(() => _started = true);
        return;
      }
      final result = _bridge.readResult(message);
      if (result != null) {
        // Toon ook het slot van de Unity-animatie voordat we teruggaan.
        _completion = Timer(const Duration(milliseconds: 1200), () {
          if (mounted) Navigator.of(context).pop(result);
        });
      }
    } on FormatException {
      // Een ander Unity-bericht is geen geldig missieresultaat.
    }
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    if (!_cameraAllowed) return;
    if (state == AppLifecycleState.resumed) {
      resumeUnity();
    } else if (state == AppLifecycleState.paused) {
      pauseUnity();
    }
  }

  @override
  void dispose() {
    _handshake?.cancel();
    _completion?.cancel();
    WidgetsBinding.instance.removeObserver(this);
    if (_cameraAllowed) {
      sendToUnity('RealityBridge', 'EndMission', '');
      pauseUnity();
    }
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppBar(title: Text(widget.glitch.name)),
    body: SafeArea(
      child: _checkingPermission
          ? const Center(child: CircularProgressIndicator())
          : _error != null
          ? Center(
              child: Padding(
                padding: const EdgeInsets.all(24),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    const Icon(Icons.error_outline, size: 48),
                    const SizedBox(height: 16),
                    Text(_error!),
                    const SizedBox(height: 16),
                    if (!_cameraAllowed) ...[
                      TextButton(
                        onPressed: openAppSettings,
                        child: const Text('Open instellingen'),
                      ),
                      FilledButton(
                        onPressed: _requestCamera,
                        child: const Text('Opnieuw proberen'),
                      ),
                    ],
                    TextButton(
                      onPressed: () => Navigator.pop(context),
                      child: const Text('Terug naar missie'),
                    ),
                  ],
                ),
              ),
            )
          : Column(
              children: [
                Expanded(child: EmbedUnity(onMessageFromUnity: _onMessage)),
                if (!_started)
                  const Padding(
                    padding: EdgeInsets.all(16),
                    child: Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        SizedBox(
                          width: 18,
                          height: 18,
                          child: CircularProgressIndicator(strokeWidth: 2),
                        ),
                        SizedBox(width: 12),
                        Text('Unity en Vuforia starten…'),
                      ],
                    ),
                  ),
              ],
            ),
    ),
  );
}
