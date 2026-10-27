import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:reality_exe/app.dart';
import 'package:reality_exe/models/player_progress.dart';
import 'package:reality_exe/models/repair_record.dart';
import 'package:reality_exe/repositories/progress_repository.dart';
import 'package:reality_exe/services/game_service.dart';

class MemoryRepository implements ProgressRepository {
  PlayerProgress progress = const PlayerProgress();
  @override
  Future<PlayerProgress> load() async => progress;
  @override
  Future<void> saveRepair(RepairRecord repair) async {
    progress = PlayerProgress(
      agentName: progress.agentName,
      repairs: [...progress.repairs, repair],
    );
  }

  @override
  Future<void> saveAgentName(String name) async {
    progress = PlayerProgress(agentName: name, repairs: progress.repairs);
  }

  @override
  Future<void> reset() async {
    progress = PlayerProgress(agentName: progress.agentName);
  }

  @override
  Future<void> close() async {}
}

void main() {
  testWidgets('Navigatie, zoeken, detail en profiel werken op telefoon', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(390, 844);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);
    final game = GameService(MemoryRepository());
    addTearDown(game.dispose);
    await tester.pumpWidget(RealityApp(game: game));
    await tester.pumpAndSettle();
    expect(find.text('De werkelijkheid is kapot.'), findsOneWidget);
    await tester.tap(find.byIcon(Icons.grid_view));
    await tester.pumpAndSettle();
    await tester.enterText(
      find.widgetWithText(TextField, 'Zoek een glitch'),
      'gravity',
    );
    await tester.pumpAndSettle();
    expect(find.text('Gravity Error'), findsOneWidget);
    tester.testTextInput.hide();
    await tester.ensureVisible(find.text('Gravity Error'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Gravity Error'));
    await tester.pumpAndSettle();
    expect(find.text('Jouw opdracht'), findsOneWidget);
    await tester.pageBack();
    await tester.pumpAndSettle();
    await tester.tap(find.byIcon(Icons.person_outline));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(TextFormField).last, 'x');
    await tester.tap(find.text('Naam opslaan'));
    await tester.pumpAndSettle();
    expect(find.text('Gebruik minstens 2 tekens.'), findsOneWidget);
    expect(tester.takeException(), isNull);
  });

  for (final size in [const Size(320, 740), const Size(1200, 900)]) {
    testWidgets('Geen layoutfouten op $size met grote tekst', (tester) async {
      tester.view.physicalSize = size;
      tester.view.devicePixelRatio = 1;
      tester.platformDispatcher.textScaleFactorTestValue = 1.8;
      addTearDown(tester.view.resetPhysicalSize);
      addTearDown(tester.view.resetDevicePixelRatio);
      addTearDown(tester.platformDispatcher.clearTextScaleFactorTestValue);
      final game = GameService(MemoryRepository());
      addTearDown(game.dispose);
      await tester.pumpWidget(RealityApp(game: game));
      await tester.pumpAndSettle();
      expect(tester.takeException(), isNull);
      final compact = find.byType(NavigationDestination);
      if (compact.evaluate().isNotEmpty) {
        await tester.tap(compact.at(2));
      } else {
        await tester.tap(
          find.descendant(
            of: find.byType(NavigationRail),
            matching: find.byIcon(Icons.center_focus_strong),
          ),
        );
      }
      await tester.pumpAndSettle();
      expect(find.text('AR-scanner'), findsOneWidget);
      // Bouw ook de kaart onderaan de scanner en controleer het assetpad.
      final scannerList = find.ancestor(
        of: find.text('AR-scanner'),
        matching: find.byType(ListView),
      );
      await tester.drag(scannerList, const Offset(0, -650));
      await tester.pumpAndSettle();
      expect(tester.takeException(), isNull);
    });
  }
}
