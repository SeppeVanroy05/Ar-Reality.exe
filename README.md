# REALITY.EXE

De werkelijkheid is kapot. Herstel drie glitches met Flutter, Unity en Vuforia.
Dit is een onderwijsproject voor twee studenten. De gewone app is gemaakt in
Flutter. **Alle 3D-objecten en alle puzzels zijn gemaakt in Unity.**

## Eerst proberen

Open deze map in VS Code. Open daarin een terminal en voer uit:

```sh
flutter pub get
flutter run -d chrome --web-port 8765
```

Je kunt nu alle Flutter-schermen bekijken. De browser heeft geen Vuforia-camera.
Er is bewust geen knop om zonder AR gratis XP te krijgen.

Voor de Unity-puzzels: open `unity/RealityUnity` met Unity Hub, versie
**6000.3.24f1**. Open `Assets/Reality/Scenes/Training.unity` en druk op Play.
Kies onderaan een missie en klik op de 3D-objecten. Deze oefenmodus bewaart
geen Flutter-voortgang en gebruikt geen cameratracking.

Voor de volledige Android-app volg je [Unity starten](docs/UNITY_STARTEN.md).
Voor de volledige iPhone-app volg je [iPhone starten](docs/IPHONE_STARTEN.md).
De database `REALITY_EXE` is aangemaakt in jullie Vuforia-account en zit al in het
Unity-project. De drie Image Targets staan op **From Database**, met hun puzzels
als child. Je hoeft deze database dus niet opnieuw aan te maken.
Een export bevat nog geen persoonlijke Vuforia-sleutel.
Voeg je eigen sleutel toe en exporteer opnieuw voordat je echte AR test.

## Wat staat er in het project?

| Map | Wat staat erin? |
| --- | --- |
| `lib/models` | Gegevens: glitch, voortgang, missieresultaat |
| `lib/data` | De drie missiebeschrijvingen |
| `lib/pages` | Dashboard, database, detail, scanner, prestaties en profiel |
| `lib/widgets` | Kleine herbruikbare delen van schermen |
| `lib/services` | Spelregels en berichten tussen Flutter en Unity |
| `lib/repositories` | SQLite op mobiel, browseropslag op web |
| `lib/theme` | Kleuren en Material 3-stijl |
| `ar_mobile` | Android-runner die dezelfde Flutter-schermen met Unity combineert |
| `unity/RealityUnity` | Het echte Unity-project met scènes en C#-scripts |
| `assets/targets` | De drie herkenningskaarten uit het lesmateriaal |
| `docs` | Uitleg, projectvoorstel, onderzoek en testresultaten |

Dit is **separation of concerns (SoC)**: elk deel heeft een duidelijke taak.
Een scherm tekent de interface; een service bevat spelregels; een repository
bewaart gegevens. Zie [Code begrijpen](docs/CODE_BEGRIJPEN.md) voor voorbeelden.

## Missies

| ID | Missie | Puzzel in Unity | Kaart | XP |
| --- | --- | --- | --- | --- |
| `glitch_001` | Mini Black Hole | Laad drie draaiende kernen op tijdens hun open tijdvenster | `stegosaurus.png` | 100 |
| `glitch_002` | Gravity Error | Speel drie steeds langere lichtcodes na | `triceratops.png` | 150 |
| `glitch_003` | Entity Escaped | Ontwijk lokvogels en raak het wezen wanneer zijn schild open is | `tyrannosaurus.png` | 200 |

De spellen hebben bewegende objecten, holografische ringen, korte geluidseffecten,
vonken bij treffers en een eigen eindanimatie. Een compacte Unity-HUD toont de
huidige opdracht en voortgang. Spelregels en effecten blijven aparte componenten.

[Download de kaartjes met de nieuwe spelregels](docs/kaartjes/REALITY_EXE_kaartjes.pdf).
De herkenningsafbeeldingen zijn hetzelfde; bestaande afdrukken blijven werken.

De dinokaarten zijn voorlopige herkenningskaarten uit de aangeleverde workshop.
De 3D-inhoud hoort bij REALITY.EXE. Je kunt later eigen glitchkaarten toevoegen.

## Zelf controleren

```sh
flutter analyze
flutter test
flutter build web
```

Controleer de Android-runner apart:

```sh
cd ar_mobile
flutter pub get
flutter analyze
flutter build apk --debug
```

De Android-build vereist een Unity-export, Android SDK 36 en NDK 28.2.13676358.
Gebruik Unity-menu `REALITY > 2 - Controleer puzzels` voor de puzzelcontroles.

## Nog nodig voor jullie indiening

- Vul de twee volledige namen in bij `docs/PROJECTVOORSTEL.md`.
- Voeg lokaal je Vuforia-sleutel toe, exporteer opnieuw en test op een Android-telefoon.
- Volg de toesteltest in `docs/TESTEN.md` en noteer het resultaat.
- Maak samen de uitleg en demonstreer dat jullie de code begrijpen.
- Werk samen via [de GitHub-repository](https://github.com/SeppeVanroy05/Ar-Reality.exe).

Er is geen Firebase-account, online login, leaderboard of cloudopslag toegevoegd.
Deze versie volgt het lokale-opslagidee uit jullie voorstel. De Flutter-interface
heeft Android-, iOS- en web-projectbestanden; de Unity-integratie heeft nu ook een iOS-buildconfiguratie in `ar_mobile`.
Op een echte iPhone zijn de camera, Vuforia-opstart en het laden van de drie Image Targets bevestigd.
Mini Black Hole verschijnt boven de stegosauruskaart. De volledige puzzelafhandeling van alle missies moet nog worden getest.
