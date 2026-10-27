import 'package:flutter/material.dart';
import '../models/glitch.dart';

// Alleen een illustratie voor de database. Het echte 3D-object staat in Unity.
class GlitchArt extends StatelessWidget {
  final Glitch glitch;
  final double height;
  const GlitchArt({super.key, required this.glitch, this.height = 180});

  static IconData iconFor(String id) {
    if (id == 'glitch_001') return Icons.blur_circular;
    if (id == 'glitch_002') return Icons.view_in_ar_outlined;
    return Icons.smart_toy_outlined;
  }

  @override
  Widget build(BuildContext context) {
    final colors = Theme.of(context).colorScheme;
    return Container(
      height: height,
      width: double.infinity,
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(16),
        gradient: RadialGradient(
          colors: [colors.primaryContainer, colors.surfaceContainerLowest],
          radius: 1,
        ),
      ),
      child: Stack(
        alignment: Alignment.center,
        children: [
          Container(
            width: 125,
            height: 125,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              border: Border.all(color: colors.primary.withValues(alpha: .3)),
            ),
          ),
          Container(
            width: 155,
            height: 155,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              border: Border.all(color: colors.primary.withValues(alpha: .12)),
            ),
          ),
          Icon(iconFor(glitch.id), size: 78, color: colors.primary),
          Positioned(
            left: 16,
            top: 14,
            child: Text(
              glitch.id.replaceAll('_', ' ').toUpperCase(),
              style: TextStyle(
                color: colors.onSurfaceVariant,
                fontSize: 11,
                letterSpacing: 1.5,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
