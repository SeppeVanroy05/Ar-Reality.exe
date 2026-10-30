# Herkomst van materiaal

- De drie herkenningsafbeeldingen zijn overgenomen uit `Image targets (1).zip`
  in het door de gebruiker aangeleverde Archive 2. Ze worden als voorlopig
  lesmateriaal gebruikt. Controleer de oorspronkelijke gebruiksrechten voordat
  jullie ze buiten de les publiceren.
- De 3D-objecten voor REALITY.EXE zijn in dit project opgebouwd uit Unity-primitives.
  De dinosaurus-, BMW-, vis- en fruitmodellen uit de archives zijn niet toegevoegd.
- `Assets/FlutterEmbed` komt uit het officiële voorbeeldproject
  `example_unity_6000_0_project` van learntoflutter/flutter_embed_unity,
  revisie `344d7fd1d3a95b98eda1005b0fdf08cfa481f90a`.
  De meegeleverde licentie staat in die map. De ARMv7-controle is aangepast zoals
  in de cursus; overige pluginbroncode is ongewijzigd.
- Vuforia 11.4.4 wordt als Unity-package opgehaald via de Git-repository uit de les,
  revisie `7df19420a497bf4bfd02dc9dfb2686a2e7012e35`.
- Flutter, Unity, Vuforia en dependencies behouden hun eigen licenties en voorwaarden.

- De database REALITY_EXE is op 27 oktober 2026 aangemaakt in het Vuforia-account
  van de gebruiker. De originele export staat in `unity/Database`; de XML en DAT
  zijn geïmporteerd onder `Assets/StreamingAssets/Vuforia`. Alle drie de targets
  zijn in Target Manager Active en hebben een fysieke breedte van 0.20 meter.

## Spelupdate 30 oktober 2026

De nieuwe drones, reactor, ringen, vonken en geluiden zijn in dit project gemaakt.
De 3D-vormen worden in de Unity-editor opgebouwd met primitives en één eigen
ringmesh. Korte geluiden worden door `MissionEffects` uit sinusgolven opgebouwd;
er zijn geen gedownloade muzieknummers of externe 3D-modellen toegevoegd.
De PDF gebruikt dezelfde oorspronkelijke dinokaarten als de database.
