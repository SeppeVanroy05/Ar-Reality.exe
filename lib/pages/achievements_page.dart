import 'package:flutter/material.dart';
import '../services/game_service.dart';
import '../widgets/page_intro.dart';

class AchievementsPage extends StatelessWidget {
  final GameService game;
  const AchievementsPage({super.key, required this.game});
  @override
  Widget build(BuildContext context) {
    final p = game.progress;
    final achievements = [
      (
        'Eerste hulp',
        'Herstel je eerste glitch.',
        p.repairedCount >= 1,
        Icons.healing,
      ),
      (
        'Zwaartekracht hersteld',
        'Los Gravity Error op.',
        p.isRepaired('glitch_002'),
        Icons.view_in_ar,
      ),
      (
        'Entity gevangen',
        'Vang het digitale wezen.',
        p.isRepaired('glitch_003'),
        Icons.smart_toy_outlined,
      ),
      (
        'Systeembeheerder',
        'Herstel alle drie de glitches.',
        p.repairedCount == 3,
        Icons.verified_outlined,
      ),
    ];
    return ListView(
      padding: const EdgeInsets.all(24),
      children: [
        PageIntro(
          label: 'AGENT RECOGNITION',
          title: 'Prestaties',
          description:
              '${achievements.where((a) => a.$3).length} van ${achievements.length} prestaties behaald.',
        ),
        ...achievements.map(
          (a) => Padding(
            padding: const EdgeInsets.only(bottom: 12),
            child: Card(
              child: Padding(
                padding: const EdgeInsets.all(8),
                child: ListTile(
                  leading: Icon(
                    a.$4,
                    size: 30,
                    color: a.$3 ? Theme.of(context).colorScheme.primary : null,
                  ),
                  title: Text(a.$1),
                  subtitle: Text(
                    '${a.$2}\n${a.$3 ? 'Behaald' : 'Nog vergrendeld'}',
                  ),
                  isThreeLine: true,
                  trailing: Icon(
                    a.$3 ? Icons.check_circle_outline : Icons.lock_outline,
                  ),
                ),
              ),
            ),
          ),
        ),
        const SizedBox(height: 12),
        const Text(
          'Prestaties volgen uit je voortgang. Je krijgt geen extra XP voor het openen van dit scherm.',
        ),
      ],
    );
  }
}
