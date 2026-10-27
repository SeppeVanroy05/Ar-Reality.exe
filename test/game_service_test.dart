import 'package:flutter_test/flutter_test.dart';
import 'package:reality_exe/models/ar_result.dart';
import 'package:reality_exe/repositories/sqlite_progress_repository.dart';
import 'package:reality_exe/services/game_service.dart';
import 'package:sqflite_common_ffi/sqflite_ffi.dart';

void main() {
  sqfliteFfiInit();
  late GameService game;
  late Database db;
  setUp(() async {
    db = await databaseFactoryFfi.openDatabase(
      inMemoryDatabasePath,
      options: OpenDatabaseOptions(
        version: 1,
        onCreate: SqliteProgressRepository.createTables,
      ),
    );
    game = GameService(SqliteProgressRepository(db));
    await game.load();
  });
  tearDown(() async {
    game.dispose();
    await db.close();
  });

  test('Missies bewaren, XP maar één keer geven en opnieuw laden', () async {
    expect(game.progress.stability, 0);
    const result = ArResult(glitchId: 'glitch_001', durationSeconds: 12);
    expect(await game.completeMission(result), isTrue);
    expect(await game.completeMission(result), isFalse);
    expect(game.progress.xp, 100);
    expect(game.progress.stability, 33);
    await game.completeMission(
      const ArResult(glitchId: 'glitch_002', durationSeconds: 20),
    );
    await game.completeMission(
      const ArResult(glitchId: 'glitch_003', durationSeconds: 30),
    );
    final reopened = GameService(SqliteProgressRepository(db));
    await reopened.load();
    expect(reopened.progress.xp, 450);
    expect(reopened.progress.level, 3);
    expect(reopened.progress.stability, 100);
    expect(reopened.progress.repairs.length, 3);
    reopened.dispose();
  });

  test('Naam opslaan, valideren en reset behoudt naam', () async {
    await game.renameAgent('  Seppe  ');
    await game.completeMission(
      const ArResult(glitchId: 'glitch_001', durationSeconds: 1),
    );
    await game.resetProgress();
    expect(game.progress.agentName, 'Seppe');
    expect(game.progress.xp, 0);
    await expectLater(game.renameAgent('x'), throwsFormatException);
  });

  test('Onbekende missie of ongeldige tijd wijzigt geen voortgang', () async {
    await expectLater(
      game.completeMission(
        const ArResult(glitchId: 'fake', durationSeconds: 5),
      ),
      throwsFormatException,
    );
    await expectLater(
      game.completeMission(
        const ArResult(glitchId: 'glitch_001', durationSeconds: -1),
      ),
      throwsFormatException,
    );
    expect(game.progress.xp, 0);
    expect(game.isSaving, isFalse);
  });

  test('Database bewaakt unieke missies ook buiten de service', () async {
    const result = ArResult(glitchId: 'glitch_001', durationSeconds: 4);
    await game.completeMission(result);
    final repair = game.progress.repairs.first;
    await game.repository.saveRepair(repair);
    expect((await game.repository.load()).repairs.length, 1);
  });
}
