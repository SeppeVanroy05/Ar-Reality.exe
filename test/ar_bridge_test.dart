import 'dart:convert';
import 'package:flutter_test/flutter_test.dart';
import 'package:reality_exe/data/glitches.dart';
import 'package:reality_exe/services/ar_bridge.dart';

void main() {
  test(
    'Alleen resultaat van huidige missie en sessie wordt één keer verwerkt',
    () {
      final bridge = ArBridge(glitches.first);
      final result = <String, Object>{
        'type': 'completed',
        'version': 1,
        'glitchId': 'glitch_001',
        'sessionId': bridge.sessionId,
        'steps': 3,
        'durationSeconds': 20,
      };
      expect(bridge.readResult('geen json'), isNull);
      expect(bridge.readResult('[]'), isNull);
      expect(
        bridge.readResult(jsonEncode({...result, 'sessionId': 'oude-sessie'})),
        isNull,
      );
      expect(
        bridge.readResult(jsonEncode({...result, 'glitchId': 'glitch_002'})),
        isNull,
      );
      expect(bridge.readResult(jsonEncode({...result, 'steps': 2})), isNull);
      expect(
        bridge.readResult(jsonEncode({...result, 'durationSeconds': -1})),
        isNull,
      );
      expect(
        bridge.readResult(jsonEncode({...result, 'durationSeconds': 2.5})),
        isNull,
      );
      expect(bridge.readResult(jsonEncode({...result, 'version': 2})), isNull);
      expect(bridge.readResult(jsonEncode(result))?.durationSeconds, 20);
      expect(bridge.readResult(jsonEncode(result)), isNull);
    },
  );

  test('Elke nieuwe missie heeft een eigen sessie', () {
    final first = ArBridge(glitches.first);
    final second = ArBridge(glitches.first);
    expect(first.sessionId, isNot(second.sessionId));
    final start = jsonDecode(first.startMessage) as Map;
    expect(start['glitchId'], 'glitch_001');
    expect(start['sessionId'], first.sessionId);
  });
}
