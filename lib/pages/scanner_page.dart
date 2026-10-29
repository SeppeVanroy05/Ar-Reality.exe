import 'package:flutter/material.dart';
import '../data/glitches.dart';
import '../models/ar_result.dart';
import '../models/glitch.dart';
import '../services/ar_page_builder.dart';
import '../services/game_service.dart';
import '../widgets/page_intro.dart';

class ScannerPage extends StatefulWidget {
  final GameService game;
  final ArPageBuilder? arPageBuilder;
  final Glitch? initialGlitch;
  const ScannerPage({
    super.key,
    required this.game,
    this.arPageBuilder,
    this.initialGlitch,
  });
  @override
  State<ScannerPage> createState() => _ScannerPageState();
}

class _ScannerPageState extends State<ScannerPage> {
  late Glitch _selected;
  bool _busy = false;
  ArResult? _pendingResult;
  String? _error;
  @override
  void initState() {
    super.initState();
    _selected = widget.initialGlitch ?? glitches.first;
  }

  Future<void> _start() async {
    setState(() {
      _busy = true;
      _error = null;
    });
    try {
      // Bij opslagfout houden we het resultaat bij. Opnieuw proberen start geen nieuwe missie.
      _pendingResult ??= await Navigator.of(context).push<ArResult>(
        MaterialPageRoute(builder: (_) => widget.arPageBuilder!(_selected)),
      );
      if (_pendingResult == null) return;
      final firstTime = await widget.game.completeMission(_pendingResult!);
      _pendingResult = null;
      if (!mounted) return;
      await showDialog<void>(
        context: context,
        builder: (context) => AlertDialog(
          icon: const Icon(Icons.check_circle_outline, size: 40),
          title: const Text('Glitch hersteld!'),
          content: Text(
            firstTime
                ? '+${_selected.xp} XP. Je voortgang is opgeslagen.'
                : 'Mooi gedaan. Je kreeg de XP voor deze missie al eerder.',
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: const Text('Verder'),
            ),
          ],
        ),
      );
    } catch (_) {
      if (mounted) {
        setState(
          () => _error = 'Opslaan of starten is niet gelukt. Probeer opnieuw.',
        );
      }
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final available = widget.arPageBuilder != null;
    return ListView(
      padding: const EdgeInsets.all(24),
      children: [
        const PageIntro(
          label: 'FIELD OPERATIONS',
          title: 'AR-scanner',
          description:
              'Breng de glitch aan het licht. Kies je missie en leg de juiste kaart klaar.',
        ),
        Card(
          child: Padding(
            padding: const EdgeInsets.all(24),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Icon(Icons.center_focus_strong, size: 64),
                const SizedBox(height: 16),
                Text(
                  available ? 'Klaar voor de scan' : 'Unity nodig voor AR',
                  style: Theme.of(context).textTheme.titleLarge,
                ),
                const SizedBox(height: 8),
                Text(
                  available
                      ? 'De camera opent in Unity. Geef cameratoegang wanneer daarom gevraagd wordt.'
                      : 'Deze uitvoering bevat geen AR-camera. Open de mobiele AR-versie om kaarten te scannen. ',
                ),
              ],
            ),
          ),
        ),
        const SizedBox(height: 24),
        DropdownButtonFormField<Glitch>(
          initialValue: _selected,
          isExpanded: true,
          decoration: const InputDecoration(labelText: 'Missie'),
          items: glitches
              .map((g) => DropdownMenuItem(value: g, child: Text(g.name)))
              .toList(),
          onChanged: _busy || _pendingResult != null
              ? null
              : (value) => setState(() => _selected = value!),
        ),
        const SizedBox(height: 20),
        Text(
          'Scan deze leskaart: ${_selected.targetFile}',
          style: Theme.of(context).textTheme.titleMedium,
        ),
        const SizedBox(height: 12),
        ClipRRect(
          borderRadius: BorderRadius.circular(16),
          child: Image.asset(
            'assets/targets/${_selected.targetFile}',
            // In de mobiele AR-versie komen deze assets uit het gedeelde pakket.
            package: widget.arPageBuilder == null ? null : 'reality_exe',
            height: 220,
            fit: BoxFit.contain,
            semanticLabel: 'Image Target voor ${_selected.name}',
          ),
        ),
        const SizedBox(height: 16),
        Text(_selected.instruction),
        const SizedBox(height: 16),
        const Text(
          '1. Toon de kaart op een tweede scherm of druk ze af.\n'
          '2. Zorg voor voldoende licht en houd de volledige kaart in beeld.\n'
          '3. Houd de kaart zichtbaar terwijl je de 3D-puzzel oplost.',
        ),
        if (_error != null)
          Padding(
            padding: const EdgeInsets.only(top: 16),
            child: Text(
              _error!,
              style: TextStyle(color: Theme.of(context).colorScheme.error),
            ),
          ),
        const SizedBox(height: 24),
        FilledButton.icon(
          onPressed: available && !_busy ? _start : null,
          icon: _busy
              ? const SizedBox(
                  width: 20,
                  height: 20,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : const Icon(Icons.view_in_ar),
          label: Text(
            _busy
                ? 'Even wachten…'
                : _pendingResult != null
                ? 'Opslaan opnieuw proberen'
                : 'Start AR-missie',
          ),
        ),
        const SizedBox(height: 12),
        const Text(
          'AR werkt op een ondersteunde telefoon. De browser ondersteunt Vuforia niet.',
        ),
      ],
    );
  }
}
