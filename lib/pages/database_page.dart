import 'package:flutter/material.dart';
import '../data/glitches.dart';
import '../models/glitch.dart';
import '../services/game_service.dart';
import '../widgets/glitch_card.dart';
import '../widgets/page_intro.dart';

class DatabasePage extends StatefulWidget {
  final GameService game;
  final ValueChanged<Glitch> onOpenGlitch;
  const DatabasePage({
    super.key,
    required this.game,
    required this.onOpenGlitch,
  });
  @override
  State<DatabasePage> createState() => _DatabasePageState();
}

class _DatabasePageState extends State<DatabasePage> {
  String _query = '';
  String _filter = 'Alle';
  @override
  Widget build(BuildContext context) {
    final found = glitches.where((g) {
      final repaired = widget.game.progress.isRepaired(g.id);
      final matchesStatus =
          _filter == 'Alle' || (_filter == 'Hersteld' ? repaired : !repaired);
      return matchesStatus &&
          '${g.name} ${g.id} ${g.description}'.toLowerCase().contains(_query);
    }).toList();
    return ListView(
      padding: const EdgeInsets.all(24),
      children: [
        const PageIntro(
          label: 'ANOMALY ARCHIVE',
          title: 'Glitch database',
          description:
              'Drie fouten in de simulatie. Drie manieren om ze te herstellen.',
        ),
        TextField(
          onChanged: (value) =>
              setState(() => _query = value.trim().toLowerCase()),
          decoration: const InputDecoration(
            labelText: 'Zoek een glitch',
            prefixIcon: Icon(Icons.search),
          ),
        ),
        const SizedBox(height: 16),
        Wrap(
          spacing: 8,
          children: ['Alle', 'Actief', 'Hersteld']
              .map(
                (value) => ChoiceChip(
                  label: Text(value),
                  selected: _filter == value,
                  onSelected: (_) => setState(() => _filter = value),
                ),
              )
              .toList(),
        ),
        const SizedBox(height: 20),
        if (found.isEmpty)
          const Padding(
            padding: EdgeInsets.all(24),
            child: Text(
              'Geen glitches gevonden. Probeer een andere zoekterm of filter.',
            ),
          ),
        ...found.map(
          (g) => Padding(
            padding: const EdgeInsets.only(bottom: 16),
            child: GlitchCard(
              glitch: g,
              repaired: widget.game.progress.isRepaired(g.id),
              onTap: () => widget.onOpenGlitch(g),
            ),
          ),
        ),
      ],
    );
  }
}
