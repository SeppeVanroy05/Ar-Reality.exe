# De app met AR op iPhone starten

De volledige mobiele app start vanuit **ar_mobile**. De Flutter-preview in de
buitenste map heeft geen Unity-camera. Je moet dus de mobiele AR-versie installeren.

## Eenmalige voorbereiding

- Gebruik een Mac met Xcode en Unity 6000.3.24f1 met iOS Build Support.
- Verbind de iPhone met de Mac en selecteer je eigen Apple-team in Xcode voor ondertekening.
- De minimale iOS-versie is 15.0 voor deze buildconfiguratie.
- Plaats je eigen Vuforia-sleutel lokaal in
  `unity/RealityUnity/Assets/Resources/vuforia-license.txt`.
- De REALITY_EXE-database en Image Targets zijn al gekoppeld zoals in de les.

## Unity-framework bouwen

Voer uit vanuit de buitenste projectmap:

```sh
./scripts/build_ios_ar.sh
```

Dit exporteert de AR-scene voor iOS en bouwt het echte UnityFramework. De gegenereerde
export en het framework blijven buiten Git. De host-app bevat de camerabeschrijving
waarmee iOS om toestemming kan vragen. De Unity-export zet Render Over Native UI aan,
zoals Vuforia voor iOS vereist.

## Flutter starten zonder debugger

```sh
cd ar_mobile
flutter pub get
flutter devices
flutter run --release -d <jouw-iphone-id>
```

Gebruik hetzelfde app-ID als de eerdere Flutter-installatie om lokale voortgang te
behouden. In deze repository is dat `be.realityexe.realityExe`.

Open daarna Scanner, kies een missie en tik op Start AR-missie. Kies Sta toe bij de
cameravraag. Toon de bijbehorende dinokaart op een tweede scherm of op papier.
Het toestaan van de camera en herkennen van de kaart moet op de echte telefoon getest
worden. Een geslaagde build bewijst nog niet dat de licentie geldig is of tracking werkt.

## Lokale compatibiliteitsaanpassing

`third_party/flutter_embed_unity_2022_3_ios` bevat versie 2.0.0 van de bestaande
Unity-plugin, met een beperkte aanpassing van de statische header-module voor Swift
Package Manager. De pluginlogica is ongewijzigd. Zie LOCAL_CHANGES.md en LICENSE in die
map. Er is geen los camerascherm dat de Unity-AR-ervaring nabootst.

## Getest op een echte iPhone

Op 29 oktober 2026 startten Flutter, Unity en de Vuforia-camera op de aangesloten
iPhone. Alle drie de Image Targets werden geladen. De stegosauruskaart werd herkend
en Mini Black Hole verscheen met drie energiepunten en de missietekst.
Het volledig uitspelen van alle drie de puzzels moet nog apart worden gecontroleerd.

Vuforia zoekt de database op iOS in de appbundel. De Xcode-configuratie neemt daarom
ook de map `Assets/StreamingAssets/Vuforia` op als Runner-resource. Alleen de kopie
in UnityFramework is onvoldoende en veroorzaakt `DATABASE_LOAD_ERROR`.
`VerifyRealityProject.ConfigureIosRunner` houdt deze instelling bij volgende exports bij.
