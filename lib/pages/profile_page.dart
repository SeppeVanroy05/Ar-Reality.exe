import 'package:flutter/material.dart';
import '../data/glitches.dart';
import '../services/game_service.dart';
import '../widgets/page_intro.dart';

class ProfilePage extends StatefulWidget {
  final GameService game;
  const ProfilePage({super.key, required this.game});
  @override
  State<ProfilePage> createState() => _ProfilePageState();
}

class _ProfilePageState extends State<ProfilePage> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _nameController;
  bool _saving = false;
  String? _error;
  @override
  void initState() {
    super.initState();
    _nameController = TextEditingController(
      text: widget.game.progress.agentName,
    );
  }

  @override
  void dispose() {
    _nameController.dispose();
    super.dispose();
  }

  Future<void> _saveName() async {
    if (!_formKey.currentState!.validate()) return;
    setState(() {
      _saving = true;
      _error = null;
    });
    try {
      await widget.game.renameAgent(_nameController.text);
      if (mounted) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(const SnackBar(content: Text('Agentnaam opgeslagen.')));
      }
    } catch (_) {
      if (mounted) {
        setState(() => _error = 'Opslaan is niet gelukt. Probeer opnieuw.');
      }
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  Future<void> _reset() async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Voortgang wissen?'),
        content: const Text(
          'Je missies, XP en prestaties worden gewist. Je agentnaam blijft behouden.',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Annuleren'),
          ),
          FilledButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text('Wissen'),
          ),
        ],
      ),
    );
    if (confirmed != true || !mounted) return;
    setState(() {
      _saving = true;
      _error = null;
    });
    try {
      await widget.game.resetProgress();
    } catch (_) {
      if (mounted) {
        setState(() => _error = 'Wissen is niet gelukt. Probeer opnieuw.');
      }
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final p = widget.game.progress;
    return ListView(
      padding: const EdgeInsets.all(24),
      children: [
        const PageIntro(
          label: 'PERSONNEL FILE',
          title: 'Jouw agent',
          description:
              'Elke herstelde glitch brengt de simulatie dichter bij normaal.',
        ),
        Card(
          child: Padding(
            padding: const EdgeInsets.all(24),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Icon(Icons.fingerprint, size: 56),
                const SizedBox(height: 16),
                Text(
                  p.agentName,
                  style: Theme.of(context).textTheme.headlineMedium,
                ),
                const SizedBox(height: 8),
                Text('Level ${p.level} · ${p.xp} XP'),
                const SizedBox(height: 16),
                LinearProgressIndicator(
                  value: p.levelProgress,
                  minHeight: 8,
                  semanticsLabel: 'Voortgang naar volgend level',
                ),
                const SizedBox(height: 8),
                Text('${200 - p.xp % 200} XP tot level ${p.level + 1}'),
              ],
            ),
          ),
        ),
        const SizedBox(height: 24),
        Form(
          key: _formKey,
          child: TextFormField(
            controller: _nameController,
            maxLength: 24,
            enabled: !_saving,
            decoration: const InputDecoration(labelText: 'Agentnaam'),
            validator: (value) => (value ?? '').trim().length < 2
                ? 'Gebruik minstens 2 tekens.'
                : null,
          ),
        ),
        if (_error != null)
          Text(
            _error!,
            style: TextStyle(color: Theme.of(context).colorScheme.error),
          ),
        FilledButton(
          onPressed: _saving ? null : _saveName,
          child: Text(_saving ? 'Even wachten…' : 'Naam opslaan'),
        ),
        const SizedBox(height: 28),
        Text(
          'Missiegeschiedenis',
          style: Theme.of(context).textTheme.titleLarge,
        ),
        const SizedBox(height: 12),
        if (p.repairs.isEmpty)
          const Text('Nog geen herstellingen. Je eerste missie wacht op je.'),
        ...p.repairs.where((r) => glitches.any((g) => g.id == r.glitchId)).map((
          r,
        ) {
          final g = glitches.firstWhere((g) => g.id == r.glitchId);
          final date = r.repairedAt.toLocal();
          return ListTile(
            contentPadding: EdgeInsets.zero,
            leading: const Icon(Icons.check_circle_outline),
            title: Text(g.name),
            subtitle: Text(
              '${date.day}/${date.month}/${date.year} · ${r.durationSeconds} seconden',
            ),
            trailing: Text('+${g.xp} XP'),
          );
        }),
        const SizedBox(height: 28),
        OutlinedButton.icon(
          onPressed: _saving ? null : _reset,
          icon: const Icon(Icons.restart_alt),
          label: const Text('Voortgang wissen'),
        ),
        const SizedBox(height: 12),
        const Text(
          'Je profiel staat lokaal op dit toestel. Er is nog geen online account of synchronisatie.',
        ),
      ],
    );
  }
}
