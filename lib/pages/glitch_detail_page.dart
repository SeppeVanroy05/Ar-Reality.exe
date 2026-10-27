import 'package:flutter/material.dart';
import '../models/glitch.dart';
import '../services/game_service.dart';
import '../services/ar_page_builder.dart';
import '../widgets/glitch_art.dart';
import 'scanner_page.dart';

class GlitchDetailPage extends StatelessWidget {
  final Glitch glitch;
  final GameService game;
  final ArPageBuilder? arPageBuilder;
  const GlitchDetailPage({
    super.key,
    required this.glitch,
    required this.game,
    this.arPageBuilder,
  });
  @override
  Widget build(BuildContext context) => ListenableBuilder(
    listenable: game,
    builder: (context, _) {
      final repaired = game.progress.isRepaired(glitch.id);
      return Scaffold(
        appBar: AppBar(title: const Text('Missiedossier')),
        body: Center(
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 800),
            child: ListView(
              padding: const EdgeInsets.all(24),
              children: [
                GlitchArt(glitch: glitch, height: 240),
                const SizedBox(height: 24),
                Text(
                  glitch.name,
                  style: Theme.of(context).textTheme.headlineLarge,
                ),
                const SizedBox(height: 12),
                Wrap(
                  spacing: 8,
                  children: [
                    Chip(label: Text(repaired ? 'Hersteld' : 'Actief')),
                    Chip(label: Text(glitch.difficulty)),
                    Chip(label: Text('${glitch.xp} XP')),
                  ],
                ),
                const SizedBox(height: 16),
                Text(
                  glitch.description,
                  style: Theme.of(context).textTheme.bodyLarge,
                ),
                const SizedBox(height: 24),
                Text(
                  'Jouw opdracht',
                  style: Theme.of(context).textTheme.titleLarge,
                ),
                const SizedBox(height: 8),
                Text(glitch.instruction),
                const SizedBox(height: 24),
                Text('Image Target: ${glitch.targetFile}'),
                const SizedBox(height: 8),
                const Text(
                  'De kaart is het anker voor het 3D-object. Bij verlies van de kaart '
                  'pauzeert de interactie tot je de kaart opnieuw in beeld brengt.',
                ),
                const SizedBox(height: 24),
                FilledButton.icon(
                  onPressed: () => Navigator.of(context).push(
                    MaterialPageRoute<void>(
                      builder: (_) => Scaffold(
                        appBar: AppBar(title: const Text('AR-missie')),
                        body: Center(
                          child: ConstrainedBox(
                            constraints: const BoxConstraints(maxWidth: 800),
                            child: ScannerPage(
                              game: game,
                              initialGlitch: glitch,
                              arPageBuilder: arPageBuilder,
                            ),
                          ),
                        ),
                      ),
                    ),
                  ),
                  icon: const Icon(Icons.center_focus_strong),
                  label: Text(
                    repaired
                        ? 'Opnieuw spelen (geen extra XP)'
                        : 'Bereid missie voor',
                  ),
                ),
              ],
            ),
          ),
        ),
      );
    },
  );
}
