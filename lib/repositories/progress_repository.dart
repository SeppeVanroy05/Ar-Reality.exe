import '../models/player_progress.dart';
import '../models/repair_record.dart';

abstract class ProgressRepository {
  Future<PlayerProgress> load();
  Future<void> saveRepair(RepairRecord repair);
  Future<void> saveAgentName(String name);
  Future<void> reset();
  Future<void> close();
}
