import 'package:flutter/material.dart';
import '../models/glitch.dart';
import 'glitch_art.dart';

class GlitchCard extends StatelessWidget {
  final Glitch glitch;
  final bool repaired;
  final VoidCallback onTap;
  const GlitchCard({
    super.key,
    required this.glitch,
    required this.repaired,
    required this.onTap,
  });
  @override
  Widget build(BuildContext context) => Card(
    clipBehavior: Clip.antiAlias,
    child: InkWell(
      onTap: onTap,
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            GlitchArt(glitch: glitch),
            const SizedBox(height: 16),
            Wrap(
              spacing: 8,
              runSpacing: 4,
              children: [
                Chip(
                  avatar: Icon(
                    repaired
                        ? Icons.check_circle_outline
                        : Icons.warning_amber_rounded,
                    size: 18,
                  ),
                  label: Text(repaired ? 'Hersteld' : 'Actief'),
                ),
                Chip(label: Text('${glitch.xp} XP')),
              ],
            ),
            const SizedBox(height: 8),
            Text(glitch.name, style: Theme.of(context).textTheme.titleLarge),
            const SizedBox(height: 8),
            Text(glitch.description),
            const SizedBox(height: 16),
            Row(
              children: [
                Expanded(child: Text(glitch.difficulty)),
                const Text('Bekijk missie'),
                const SizedBox(width: 8),
                const Icon(Icons.arrow_forward, size: 18),
              ],
            ),
          ],
        ),
      ),
    ),
  );
}
