// Een model beschrijft gegevens. Het tekent zelf geen widgets.
class Glitch {
  final String id;
  final String name;
  final String description;
  final String difficulty;
  final String instruction;
  final String targetFile;
  final int xp;
  final int steps;

  const Glitch({
    required this.id,
    required this.name,
    required this.description,
    required this.difficulty,
    required this.instruction,
    required this.targetFile,
    required this.xp,
    required this.steps,
  });
}
