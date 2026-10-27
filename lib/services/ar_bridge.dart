import 'dart:convert';
import 'dart:math';
import '../models/ar_result.dart';
import '../models/glitch.dart';

// Klein berichtcontract, los van de Unity-plugin. Daardoor kunnen we dit testen.
class ArBridge {
  final Glitch glitch;
  final String sessionId;
  bool _completed = false;
  ArBridge(this.glitch) : sessionId = _newSessionId();

  static String _newSessionId() {
    final random = Random.secure();
    return List.generate(
      16,
      (_) => random.nextInt(256).toRadixString(16).padLeft(2, '0'),
    ).join();
  }

  String get startMessage => jsonEncode({
    'type': 'start',
    'version': 1,
    'glitchId': glitch.id,
    'sessionId': sessionId,
  });

  ArResult? readResult(String message) {
    try {
      final data = jsonDecode(message);
      if (data is! Map<String, dynamic>) return null;
      if (_completed ||
          data['type'] != 'completed' ||
          data['version'] != 1 ||
          data['glitchId'] != glitch.id ||
          data['sessionId'] != sessionId ||
          data['steps'] != glitch.steps ||
          data['durationSeconds'] is! int) {
        return null;
      }
      final seconds = data['durationSeconds'] as int;
      if (seconds < 0 || seconds > 86400) return null;
      _completed = true;
      return ArResult(glitchId: glitch.id, durationSeconds: seconds);
    } on FormatException {
      return null;
    }
  }
}
