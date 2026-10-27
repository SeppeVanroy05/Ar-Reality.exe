import 'package:path/path.dart' as path;
import 'package:sqflite/sqflite.dart';
import '../models/player_progress.dart';
import '../models/repair_record.dart';
import 'progress_repository.dart';

class SqliteProgressRepository implements ProgressRepository {
  final Database database;
  SqliteProgressRepository(this.database);

  static Future<SqliteProgressRepository> open() async {
    final location = path.join(await getDatabasesPath(), 'reality_exe.db');
    final db = await openDatabase(location, version: 1, onCreate: createTables);
    return SqliteProgressRepository(db);
  }

  static Future<void> createTables(Database db, int version) async {
    await db.execute(
      'CREATE TABLE repairs ('
      'glitch_id TEXT PRIMARY KEY, repaired_at TEXT NOT NULL, '
      'duration_seconds INTEGER NOT NULL CHECK(duration_seconds >= 0))',
    );
    await db.execute(
      'CREATE TABLE settings (name TEXT PRIMARY KEY, value TEXT NOT NULL)',
    );
  }

  @override
  Future<PlayerProgress> load() async {
    final rows = await database.query('repairs', orderBy: 'repaired_at DESC');
    final names = await database.query(
      'settings',
      where: 'name = ?',
      whereArgs: ['agent_name'],
    );
    return PlayerProgress(
      agentName: names.isEmpty ? 'Agent 001' : names.first['value'] as String,
      repairs: rows.map(RepairRecord.fromMap).toList(),
    );
  }

  @override
  Future<void> saveRepair(RepairRecord repair) async {
    // PRIMARY KEY + ignore: dezelfde missie geeft nooit tweemaal XP.
    await database.insert(
      'repairs',
      repair.toMap(),
      conflictAlgorithm: ConflictAlgorithm.ignore,
    );
  }

  @override
  Future<void> saveAgentName(String name) async {
    await database.insert('settings', {
      'name': 'agent_name',
      'value': name,
    }, conflictAlgorithm: ConflictAlgorithm.replace);
  }

  @override
  Future<void> reset() async {
    // Profielnaam blijft behouden. Alleen spelvoortgang wordt gewist.
    await database.delete('repairs');
  }

  @override
  Future<void> close() => database.close();
}
