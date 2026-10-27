import 'package:flutter/foundation.dart';
import '../data/glitches.dart';
import '../models/ar_result.dart';
import '../models/player_progress.dart';
import '../models/repair_record.dart';
import '../repositories/progress_repository.dart';

// Gedeelde spelregels. Geen databasecode en geen widgets in deze klasse.
class GameService extends ChangeNotifier {
  final ProgressRepository repository;
  PlayerProgress _progress = const PlayerProgress();
  bool _isSaving = false;
  PlayerProgress get progress => _progress;
  bool get isSaving => _isSaving;

  GameService(this.repository);

  Future<void> load() async {
    _progress = await repository.load();
    notifyListeners();
  }

  Future<bool> completeMission(ArResult result) async {
    if (_isSaving) {
      throw StateError('Er worden al gegevens opgeslagen. Probeer opnieuw.');
    }
    if (!glitches.any((g) => g.id == result.glitchId) ||
        result.durationSeconds < 0 ||
        result.durationSeconds > 86400) {
      throw const FormatException('Ongeldig missieresultaat.');
    }
    if (_progress.isRepaired(result.glitchId)) return false;
    _isSaving = true;
    notifyListeners();
    try {
      await repository.saveRepair(
        RepairRecord(
          glitchId: result.glitchId,
          repairedAt: DateTime.now(),
          durationSeconds: result.durationSeconds,
        ),
      );
      await load();
      return true;
    } finally {
      _isSaving = false;
      notifyListeners();
    }
  }

  Future<void> renameAgent(String value) async {
    final name = value.trim();
    if (name.length < 2 || name.length > 24) {
      throw const FormatException('Gebruik 2 tot 24 tekens.');
    }
    await repository.saveAgentName(name);
    await load();
  }

  Future<void> resetProgress() async {
    await repository.reset();
    await load();
  }
}
