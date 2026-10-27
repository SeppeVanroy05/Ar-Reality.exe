import 'package:flutter/material.dart';
import 'pages/home_page.dart';
import 'repositories/open_repository.dart';
import 'services/game_service.dart';
import 'services/ar_page_builder.dart';
import 'theme/app_theme.dart';

class RealityApp extends StatefulWidget {
  final ArPageBuilder? arPageBuilder;
  final GameService? game;
  const RealityApp({super.key, this.arPageBuilder, this.game});
  @override
  State<RealityApp> createState() => _RealityAppState();
}

class _RealityAppState extends State<RealityApp> {
  GameService? _game;
  String? _error;
  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _error = null);
    GameService? created;
    try {
      created = widget.game ?? GameService(await openRepository());
      await created.load();
      if (!mounted) {
        if (widget.game == null) {
          await created.repository.close();
          created.dispose();
        }
        return;
      }
      setState(() => _game = created);
    } catch (_) {
      if (created != null && widget.game == null) {
        await created.repository.close();
        created.dispose();
      }
      if (mounted) {
        setState(
          () =>
              _error = 'Je voortgang kon niet worden geladen. Probeer opnieuw.',
        );
      }
    }
  }

  @override
  void dispose() {
    if (widget.game == null && _game != null) {
      _game!.repository.close();
      _game!.dispose();
    }
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => MaterialApp(
    title: 'REALITY.EXE',
    debugShowCheckedModeBanner: false,
    theme: AppTheme.create(Brightness.light),
    darkTheme: AppTheme.create(Brightness.dark),
    themeMode: ThemeMode.dark,
    home: _game != null
        ? HomePage(game: _game!, arPageBuilder: widget.arPageBuilder)
        : Scaffold(
            body: Center(
              child: Padding(
                padding: const EdgeInsets.all(24),
                child: _error == null
                    ? const CircularProgressIndicator()
                    : Column(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Text(_error!),
                          const SizedBox(height: 16),
                          FilledButton(
                            onPressed: _load,
                            child: const Text('Opnieuw proberen'),
                          ),
                        ],
                      ),
              ),
            ),
          ),
  );
}
