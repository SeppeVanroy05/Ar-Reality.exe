import '../data/glitches.dart';
import 'repair_record.dart';

class PlayerProgress {
  final String agentName;
  final List<RepairRecord> repairs;

  const PlayerProgress({this.agentName = 'Agent 001', this.repairs = const []});

  bool isRepaired(String id) => repairs.any((repair) => repair.glitchId == id);
  // Afleiden voorkomt dat XP en status elkaar kunnen tegenspreken.
  int get xp => glitches
      .where((g) => isRepaired(g.id))
      .fold(0, (total, glitch) => total + glitch.xp);
  int get repairedCount => glitches.where((g) => isRepaired(g.id)).length;
  int get level => 1 + xp ~/ 200;
  double get levelProgress => (xp % 200) / 200;
  int get stability => (repairedCount / glitches.length * 100).round();
}
