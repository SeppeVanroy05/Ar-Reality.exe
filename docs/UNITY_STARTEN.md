# Unity en de Android-app starten

## Wat heb je nodig?

- Flutter **3.44.0** en Dart **3.12**.
- Unity Hub en Unity **6000.3.24f1**, met Android Build Support, SDK, NDK en OpenJDK.
- Android SDK **36** en NDK **28.2.13676358** voor Flutter.
- Een eigen Vuforia Engine-licentiesleutel.
- Een echte Android-telefoon met camera; de app heeft minimaal Android 10 nodig.

Dit volgt de versiecombinatie van de cursus. Daarnaast is in de Android-runner
ook `flutter_embed_unity_6000_0_android: 2.0.0` opgenomen. De plugin vereist die
aparte dependency om Unity 6 op Android te ondersteunen.

## Stap 1 — Open het Unity-project

1. Open Unity Hub en kies Add > Add project from disk.
2. Selecteer `unity/RealityUnity`, niet de buitenste Flutter-map.
3. Open met Unity 6000.3.24f1.
4. Laat de packages importeren. Vuforia 11.4.4 komt uit dezelfde Git-repository als in de les.
5. Controleer dat de Console geen rode compileerfouten bevat.

De Flutter Embed-exportplugin staat al in `Assets/FlutterEmbed`.
De ARMv7-controle is aangepast zoals in de cursus, omdat we alleen ARM64 gebruiken.

## Stap 2 — Probeer de puzzels zonder camera

1. Open `Assets/Reality/Scenes/Training.unity`.
2. Druk op Play en kies in Game View een portretformaat, bijvoorbeeld 540 × 960.
3. Volg de oplichtende kern. Klik tweemaal tijdens het mintgroene tijdvenster; verzegel drie kernen.
4. Kies Missie 2, onthoud de lichtcode en speel hem na. Elke ronde wordt langer; Code herhalen speelt de huidige ronde opnieuw.
5. Kies Missie 3, vermijd de rode lokvogels en vang het mintgroene wezen driemaal wanneer het paarse schild open is.

De oefenmodus gebruikt Unity, maar geen Vuforia. Er worden geen resultaten naar
Flutter gestuurd. Stop Play voordat je scènes of instellingen wijzigt.

## Stap 3 — Voeg je eigen licentiesleutel toe

Maak lokaal dit tekstbestand:

```text
unity/RealityUnity/Assets/Resources/vuforia-license.txt
```

Zet er alleen je eigen Vuforia-sleutel in, zonder aanhalingstekens. Dit bestand
en de `.meta` staan in `.gitignore`. Plak je sleutel niet in broncode of README.

`ArBootstrap` leest dit bestand en stelt Vuforia in voordat de AR-camera start.
Zonder sleutel toont de mobiele app een foutmelding. Een export zonder sleutel
is nuttig als buildcontrole, maar kan geen Vuforia-tracking uitvoeren.

## Stap 4 — Image Targets via Target Manager

De drie PNG's staan in `Assets/Reality/Targets`. De volgorde is:

1. stegosaurus → Mini Black Hole;
2. triceratops → Gravity Error;
3. tyrannosaurus → Entity Escaped.

**Dit is al uitgevoerd:** de database REALITY_EXE is aangemaakt, de drie targets
zijn Active en de database is geïmporteerd en aan de scène gekoppeld. De originele
Unity-package staat in `unity/Database/REALITY_EXE.unitypackage`.

Hieronder staat hoe deze instellingen volgens de cursus zijn gemaakt, voor als
je later zelf een kaart wilt vervangen:

1. Open Vuforia Target Manager en maak een database **REALITY_EXE** van type **Device**.
2. Voeg drie targets toe van type **Image**:

| Afbeelding | Naam in Target Manager | Breedte |
| --- | --- | --- |
| `stegosaurus.png` | `glitch_001` | 0.20 |
| `triceratops.png` | `glitch_002` | 0.20 |
| `tyrannosaurus.png` | `glitch_003` | 0.20 |

3. Wacht tot de targets **Active** zijn. Bekijk ook de herkenningskwaliteit.
4. Download de database voor **Unity Editor** en importeer de `.unitypackage` in Unity.
5. Open `Assets/Reality/Scenes/RealityAR.unity`.
6. Selecteer `ImageTarget_glitch_001` in de Hierarchy.
7. Kies in **Image Target Behaviour**: **Type: From Database**, **Database: REALITY_EXE**,
   **Image Target: glitch_001**.
8. Herhaal voor `ImageTarget_glitch_002` en `ImageTarget_glitch_003` met hun eigen targetnamen.
9. De juiste 3D-puzzel staat al als child onder elk Image Target.
10. Kies **REALITY > 4 - Controleer Image Target-database** en sla de scène op.

De opgegeven breedte van 0.20 betekent **20 cm** voor de afbeelding zelf,
niet voor het papier errond. Print zonder de verhouding van de afbeelding te veranderen.
Gebruik je een andere fysieke breedte, pas dan de breedte in de targetinstellingen aan.

De Image Targets worden niet meer via `ObserverFactory` tijdens het spelen aangemaakt.
De scène bevat echte, bewerkbare Vuforia Image Target-componenten. De exportcontrole
blokkeert een mobiele build wanneer de database of koppelingen ontbreken.

## Stap 5 — Controleer Android-instellingen

Open `Assets/Reality/Scenes/RealityAR.unity`. Via File > Build Profiles:

- Switch Platform naar Android.
- Alleen `RealityAR.unity` staat in de build-scènelijst.
- Scripting Backend: IL2CPP.
- Target Architectures: alleen ARM64.
- Minimum API: 29. Target API: 36.
- Application Entry Point: Activity.
- Graphics API: alleen OpenGLES3.
- Export Project: aan.

Deze projectinstellingen zijn al voorbereid. De Training-scène mag niet mee
in de mobiele export. Gebruik `REALITY > 1 - Maak scenes opnieuw` alleen als je
de oorspronkelijke scènes wilt herstellen: dit overschrijft scène-aanpassingen.

## Stap 6 — Exporteer Unity voor Flutter

Gebruik het menu **Flutter Embed > Export project to Android**.
Kies als doelmap:

```text
ar_mobile/android/unityLibrary
```

De exporter kan vragen een bestaande export te vervangen. Dit betreft alleen
de gegenereerde export, niet de C#-broncode of scènes.

Voer daarna vanuit de buitenste projectmap uit:

```sh
python3 scripts/prepare_unity_export.py
```

Dit kleine script verwijdert `-ignorewarnings` uit de Unity ProGuard-instellingen
en stemt de NDK-versie af op de cursus. De Gradle-configuratie neemt
`xrmanifest.androidlib` alleen op wanneer Unity die werkelijk exporteert.
Onze Vuforia-opzet gebruikt geen AR Foundation en heeft die submodule niet nodig.

## Stap 7 — Start op je telefoon

Vanuit de buitenste projectmap:

```sh
cd ar_mobile
flutter pub get
flutter devices
flutter run -d <jouw-android-device-id>
```

Sta cameratoegang toe. Kies een missie, toon de kaart op een tweede scherm of
op papier, en speel de puzzel. Controleer na afloop of de XP en status in
Flutter veranderen en na een herstart bewaard blijven.

## Veelvoorkomende problemen

| Probleem | Controle |
| --- | --- |
| unityLibrary ontbreekt | Exporteer eerst Unity naar de genoemde map |
| Camera geweigerd | Open de toestelinstellingen via de melding en geef toestemming |
| Vuforia-sleutel ontbreekt | Voeg het lokale sleutelbestand toe en exporteer opnieuw |
| Kaart wordt niet herkend | Kies de juiste afbeelding, zorg voor licht, houd de hele kaart zichtbaar |
| NDK niet gevonden | Installeer 28.2.13676358 via Android Studio > SDK Manager |
| Unity reageert niet | Controleer scènes, export, Console en GameObjectnaam RealityBridge |
| Opnieuw spelen geeft geen XP | Dit is de bedoelde regel; alleen de eerste herstelling telt |

De echte cameraketen is nog niet getest met jullie sleutel en telefoon.
Noteer die test in `TESTEN.md`. De iOS-map in de gewone Flutter-app betekent
niet dat de Unity-AR-koppeling voor iOS al af is.
