import 'package:flutter/material.dart';
import '../data/glitches.dart';
import '../models/glitch.dart';
import '../services/game_service.dart';
import '../widgets/page_intro.dart';
import '../widgets/glitch_card.dart';

class DashboardPage extends StatelessWidget {
  final GameService game;
  final ValueChanged<Glitch> onOpenGlitch;
  final VoidCallback onOpenDatabase;
  const DashboardPage({
    super.key,
    required this.game,
    required this.onOpenGlitch,
    required this.onOpenDatabase,
  });
  @override
  Widget build(BuildContext context) {
    final progress = game.progress;
    final active = glitches.where((g) => !progress.isRepaired(g.id)).toList();
    final theme = Theme.of(context);
    return ListView(
      padding: const EdgeInsets.all(24),
      children: [
        PageIntro(
          label: 'SIMULATION CONTROL',
          title: 'De werkelijkheid is kapot.',
          description:
              'Welkom, ${progress.agentName}. '
              '${active.isEmpty ? 'Alle glitches zijn hersteld. Goed werk.' : 'Er wachten ${active.length} glitches op herstel.'}',
        ),
        Card(
          child: Padding(
            padding: const EdgeInsets.all(24),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Row(
                  children: [
                    Icon(Icons.public),
                    SizedBox(width: 10),
                    Text('WERELDSTABILITEIT'),
                  ],
                ),
                const SizedBox(height: 20),
                Text(
                  '${progress.stability}%',
                  style: theme.textTheme.displayLarge?.copyWith(
                    fontWeight: FontWeight.bold,
                    color: theme.colorScheme.primary,
                  ),
                ),
                const SizedBox(height: 12),
                LinearProgressIndicator(
                  value: progress.stability / 100,
                  minHeight: 10,
                  borderRadius: BorderRadius.circular(8),
                  semanticsLabel: 'Wereldstabiliteit',
                  semanticsValue: '${progress.stability}',
                ),
                const SizedBox(height: 16),
                Text(
                  '${progress.repairedCount} van ${glitches.length} glitches hersteld',
                ),
                const SizedBox(height: 24),
                Wrap(
                  spacing: 12,
                  runSpacing: 12,
                  children: [
                    Chip(
                      avatar: const Icon(Icons.bolt, size: 18),
                      label: Text('${progress.xp} XP'),
                    ),
                    Chip(
                      avatar: const Icon(Icons.shield_outlined, size: 18),
                      label: Text('Level ${progress.level}'),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ),
        const SizedBox(height: 28),
        Wrap(
          alignment: WrapAlignment.spaceBetween,
          crossAxisAlignment: WrapCrossAlignment.center,
          spacing: 16,
          children: [
            Text('Volgende missie', style: theme.textTheme.titleLarge),
            TextButton(
              onPressed: onOpenDatabase,
              child: const Text('Alle glitches →'),
            ),
          ],
        ),
        const SizedBox(height: 12),
        if (active.isNotEmpty)
          GlitchCard(
            glitch: active.first,
            repaired: false,
            onTap: () => onOpenGlitch(active.first),
          )
        else
          Card(
            child: Padding(
              padding: const EdgeInsets.all(24),
              child: Column(
                children: [
                  const Icon(Icons.verified_outlined, size: 48),
                  const SizedBox(height: 12),
                  const Text(
                    'Simulatie hersteld. Je kunt missies opnieuw spelen.',
                  ),
                  TextButton(
                    onPressed: onOpenDatabase,
                    child: const Text('Bekijk missies'),
                  ),
                ],
              ),
            ),
          ),
        const SizedBox(height: 24),
        Text('BRIEFING', style: theme.textTheme.labelLarge),
        const SizedBox(height: 8),
        const Text(
          'De wereld is een simulatie. Jij werkt voor de organisatie die de fouten '
          'herstelt. Kies een missie, scan de bijbehorende kaart en los de glitch op in Unity AR.',
        ),
      ],
    );
  }
}
