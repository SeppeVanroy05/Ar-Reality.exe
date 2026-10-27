import 'package:flutter/material.dart';
import '../models/glitch.dart';
import '../services/game_service.dart';
import '../services/ar_page_builder.dart';
import 'dashboard_page.dart';
import 'database_page.dart';
import 'scanner_page.dart';
import 'achievements_page.dart';
import 'profile_page.dart';
import 'glitch_detail_page.dart';

class HomePage extends StatefulWidget {
  final GameService game;
  final ArPageBuilder? arPageBuilder;
  const HomePage({super.key, required this.game, this.arPageBuilder});
  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  int _selectedIndex = 0;
  final _labels = ['Dashboard', 'Glitches', 'Scanner', 'Prestaties', 'Profiel'];
  final _icons = [
    Icons.dashboard_outlined,
    Icons.grid_view,
    Icons.center_focus_strong,
    Icons.emoji_events_outlined,
    Icons.person_outline,
  ];

  void _openGlitch(Glitch glitch) => Navigator.of(context).push(
    MaterialPageRoute<void>(
      builder: (_) => GlitchDetailPage(
        glitch: glitch,
        game: widget.game,
        arPageBuilder: widget.arPageBuilder,
      ),
    ),
  );

  @override
  Widget build(BuildContext context) => ListenableBuilder(
    listenable: widget.game,
    builder: (context, _) {
      final pages = <Widget>[
        DashboardPage(
          game: widget.game,
          onOpenGlitch: _openGlitch,
          onOpenDatabase: () => setState(() => _selectedIndex = 1),
        ),
        DatabasePage(game: widget.game, onOpenGlitch: _openGlitch),
        ScannerPage(game: widget.game, arPageBuilder: widget.arPageBuilder),
        AchievementsPage(game: widget.game),
        ProfilePage(game: widget.game),
      ];
      return LayoutBuilder(
        builder: (context, constraints) {
          final wide = constraints.maxWidth >= 720;
          final content = Center(
            child: ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 900),
              child: IndexedStack(index: _selectedIndex, children: pages),
            ),
          );
          return Scaffold(
            appBar: AppBar(
              title: const Text(
                'REALITY.EXE',
                style: TextStyle(fontWeight: FontWeight.w900, letterSpacing: 2),
              ),
              actions: [
                Padding(
                  padding: const EdgeInsets.only(right: 16),
                  child: Chip(
                    avatar: Icon(
                      widget.arPageBuilder == null
                          ? Icons.code
                          : Icons.view_in_ar,
                      size: 16,
                    ),
                    label: Text(
                      widget.arPageBuilder == null ? 'Flutter' : 'Unity AR',
                    ),
                  ),
                ),
              ],
            ),
            body: SafeArea(
              child: wide
                  ? Row(
                      children: [
                        NavigationRail(
                          selectedIndex: _selectedIndex,
                          labelType: NavigationRailLabelType.all,
                          onDestinationSelected: (value) =>
                              setState(() => _selectedIndex = value),
                          destinations: List.generate(
                            _labels.length,
                            (i) => NavigationRailDestination(
                              icon: Icon(_icons[i]),
                              label: Text(_labels[i]),
                            ),
                          ),
                        ),
                        const VerticalDivider(width: 1),
                        Expanded(child: content),
                      ],
                    )
                  : content,
            ),
            bottomNavigationBar: wide
                ? null
                : NavigationBar(
                    selectedIndex: _selectedIndex,
                    onDestinationSelected: (value) =>
                        setState(() => _selectedIndex = value),
                    destinations: List.generate(
                      _labels.length,
                      (i) => NavigationDestination(
                        icon: Icon(_icons[i]),
                        label: _labels[i],
                      ),
                    ),
                  ),
          );
        },
      );
    },
  );
}
