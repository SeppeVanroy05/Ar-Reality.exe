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
3. Klik op de energiepunten. Eén punt tweemaal aanklikken telt maar één keer.
4. Kies Missie 2 en klik op de blokken 3 → 1 → 2.
5. Kies Missie 3 en klik drie keer op het bewegende wezen.

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

## Stap 4 — Begrijp de Image Targets

De drie PNG's staan in `Assets/Reality/Targets`. De volgorde is:

1. stegosaurus → Mini Black Hole;
2. triceratops → Gravity Error;
3. tyrannosaurus → Entity Escaped.

In de les voeg je Image Targets vaak via Target Manager en een database toe.
Hier maakt `ArBootstrap` ze met `ObserverFactory.CreateImageTarget` rechtstreeks
uit dezelfde afbeeldingen. Het blijft echte Vuforia Image Target Tracking.
Dit bespaart de handmatige database-upload. Je hoeft geen .dat-bestand te maken.

`Read/Write` staat aan voor deze textures en compressie staat uit. De fysieke
kaartbreedte is **20 cm**. Stel `printedWidthMeters` op het RealityBridge-object
in de AR-scène anders in als je een andere breedte gebruikt. Meet de afbeelding
zelf, niet het papier errond. Print met behoud van de oorspronkelijke verhouding.
Bij een ander schermformaat kan de grootte van de virtuele inhoud afwijken.

De bronafbeeldingen staan ook in de Flutter-map `assets/targets`.

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
