# Vuforia-database REALITY_EXE

De database is aangemaakt in het Vuforia-account van de gebruiker, met toestemming
om de database en targets te maken. Bestaande lesdatabases zijn behouden.

[Open REALITY_EXE in Target Manager](https://developer.vuforia.com/develop/databases/47bdc9c53d7e4554b249496ce7c85966/targets)

| Targetnaam | Afbeelding | Afmeting in meters (breedte × hoogte) | Missie | Status in Target Manager |
| --- | --- | --- | --- | --- |
| glitch_001 | stegosaurus.png | 0.200000 × 0.121942 | Mini Black Hole | Active |
| glitch_002 | triceratops.png | 0.200000 × 0.119560 | Gravity Error | Active |
| glitch_003 | tyrannosaurus.png | 0.200000 × 0.123146 | Entity Escaped | Active |

Status gecontroleerd op 27 oktober 2026. De breedte van de afbeelding op papier
moet 20 cm zijn. Houd de oorspronkelijke verhouding aan.

De originele download voor Unity Editor staat in
`unity/Database/REALITY_EXE.unitypackage`. De geïmporteerde database staat in:

- `unity/RealityUnity/Assets/StreamingAssets/Vuforia/REALITY_EXE.xml`
- `unity/RealityUnity/Assets/StreamingAssets/Vuforia/REALITY_EXE.dat`

De preview-afbeeldingen uit de officiële export staan in
`Assets/Editor/Vuforia/ImageTargetTextures/REALITY_EXE`.

In `RealityAR.unity` staan de drie Image Targets met **Type: From Database**,
**Database: REALITY_EXE** en hun eigen targetnaam. De juiste puzzel staat onder
elk target. Er worden geen Image Targets vanuit losse textures aangemaakt tijdens
het spelen.

De Unity-exportcontrole controleert de componenten, child-puzzels en aanwezigheid
van de XML en DAT. De Vuforia-licentiesleutel moet nog lokaal toegevoegd worden
zoals uitgelegd in `UNITY_STARTEN.md`. Camera- en herkenningstests op een echte
telefoon zijn nog niet uitgevoerd.
