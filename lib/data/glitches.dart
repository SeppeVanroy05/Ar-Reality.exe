import '../models/glitch.dart';

// Dezelfde IDs staan in Unity. Verander ze dus niet aan één kant alleen.
const glitches = <Glitch>[
  Glitch(
    id: 'glitch_001',
    name: 'Mini Black Hole',
    description:
        'Een zwart gat slokt stukjes van de simulatie op. '
        'Activeer de drie energiepunten om het te sluiten.',
    difficulty: 'Makkelijk',
    instruction:
        'Tik op de drie verschillende energiepunten rond het zwarte gat.',
    targetFile: 'stegosaurus.png',
    xp: 100,
    steps: 3,
  ),
  Glitch(
    id: 'glitch_002',
    name: 'Gravity Error',
    description:
        'Een kubus hangt vast in de lucht. De zwaartekrachtcode '
        'is verwisseld. Herstel de code om de kubus te laten landen.',
    difficulty: 'Gemiddeld',
    instruction:
        'Tik op de knoppen in deze volgorde: 3 → 1 → 2. Een fout begint de reeks opnieuw.',
    targetFile: 'triceratops.png',
    xp: 150,
    steps: 3,
  ),
  Glitch(
    id: 'glitch_003',
    name: 'Entity Escaped',
    description:
        'Een digitaal wezen is uit een portaal ontsnapt. '
        'Het springt van plaats naar plaats. Vang het drie keer.',
    difficulty: 'Gemiddeld',
    instruction:
        'Tik drie keer op het bewegende wezen. Na elke tik verplaatst het zich.',
    targetFile: 'tyrannosaurus.png',
    xp: 200,
    steps: 3,
  ),
];
