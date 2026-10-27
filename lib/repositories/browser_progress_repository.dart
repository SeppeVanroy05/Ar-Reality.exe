import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import '../models/player_progress.dart';
import '../models/repair_record.dart';
import 'progress_repository.dart';

// SQLite is voor de mobiele app. De browser bewaart dezelfde gegevens als JSON.
class BrowserProgressRepository implements ProgressRepository {
  final SharedPreferencesAsync preferences;
  BrowserProgressRepository(this.preferences);
  static Future<BrowserProgressRepository> open() async =>
      BrowserProgressRepository(SharedPreferencesAsync());

  @override
  Future<PlayerProgress> load() async {
    final stored = await preferences.getString('reality_repairs');
    final rows = stored == null ? <dynamic>[] : jsonDecode(stored) as List;
    return PlayerProgress(
      agentName: await preferences.getString('reality_agent') ?? 'Agent 001',
      repairs: rows
          .map(
            (row) =>
                RepairRecord.fromMap(Map<String, dynamic>.from(row as Map)),
          )
          .toList(),
    );
  }

  @override
  Future<void> saveRepair(RepairRecord repair) async {
    final progress = await load();
    if (progress.isRepaired(repair.glitchId)) return;
    await preferences.setString(
      'reality_repairs',
      jsonEncode([repair.toMap(), ...progress.repairs.map((r) => r.toMap())]),
    );
  }

  @override
  Future<void> saveAgentName(String name) =>
      preferences.setString('reality_agent', name);
  @override
  Future<void> reset() => preferences.remove('reality_repairs');
  @override
  Future<void> close() async {}
}
