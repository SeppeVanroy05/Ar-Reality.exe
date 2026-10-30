import '../models/glitch.dart';

// Dezelfde IDs staan in Unity. Verander ze dus niet aan één kant alleen.
const glitches = <Glitch>[
  Glitch(
    id: 'glitch_001',
    name: 'Mini Black Hole',
    description:
        'Een zwart gat slokt stukjes van de simulatie op. '
        'Laad de draaiende energiekernen op om het te sluiten.',
    difficulty: 'Makkelijk',
    instruction:
        'Laad elke kern tweemaal wanneer hij mintgroen oplicht. Volg de actieve kern en verzegel ze alle drie.',
    targetFile: 'stegosaurus.png',
    xp: 100,
    steps: 3,
  ),
  Glitch(
    id: 'glitch_002',
    name: 'Gravity Error',
    description:
        'Een reactor zweeft in losse stukken. De zwaartekrachtcode '
        'is beschadigd. Onthoud drie lichtcodes om de reactor te herstellen.',
    difficulty: 'Gemiddeld',
    instruction:
        'Kijk naar de oplichtende knoppen en speel de code na. Elke ronde wordt langer. Gebruik Code herhalen als je het even kwijt bent.',
    targetFile: 'triceratops.png',
    xp: 150,
    steps: 3,
  ),
  Glitch(
    id: 'glitch_003',
    name: 'Entity Escaped',
    description:
        'Een digitaal wezen is uit een portaal ontsnapt. '
        'Het ontwijkt je tussen rode lokvogels. Doorbreek zijn schild en vang het.',
    difficulty: 'Gemiddeld',
    instruction:
        'Raak het wezen met de mintgroene ogen als zijn paarse schild open is. Vermijd rode lokvogels en laad het vangstveld drie keer.',
    targetFile: 'tyrannosaurus.png',
    xp: 200,
    steps: 3,
  ),
];
