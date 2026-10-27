# Projectvoorstel — REALITY.EXE

## Teamleden

| Naam | GitHub-gebruikersnaam |
| --- | --- |
| [Volledige naam invullen] | nickhsm |
| [Volledige naam invullen] | SeppeVanroy05 |

## Thema en onderwerp

REALITY.EXE is een mobiele AR-game. De echte wereld blijkt een computersimulatie
waarin fouten ontstaan. De speler is een agent die deze glitches onderzoekt en
herstelt. Het controlecentrum en de voortgang staan in Flutter. De speler scant
een fysieke kaart om een interactieve 3D-puzzel in Unity te openen.

De eerste versie bevat drie missies: een klein zwart gat stabiliseren, een
zwaartekrachtcode herstellen en een ontsnapt digitaal wezen vangen.

## Noodzakelijke pagina's

1. **Dashboard:** wereldstabiliteit, XP, level en volgende actieve missie.
2. **Glitch database:** alle glitches met zoeken en filters voor actief/hersteld.
3. **Glitch details:** verhaal, opdracht, moeilijkheid, beloning en bijbehorende kaart.
4. **AR-scanner:** missie kiezen, kaart tonen en de Unity-camera openen.
5. **Prestaties:** vrijgespeelde en nog vergrendelde achievements.
6. **Profiel:** agentnaam wijzigen, voortgang en missiegeschiedenis bekijken.

## AR flavor en meerwaarde

We gebruiken **marker-based AR met Vuforia Image Targets in Unity**. De kaart
vormt het anker voor een 3D-object. De speler tikt op echte 3D-colliders via
raycasting en lost een puzzel op terwijl de kaart in beeld blijft.

De meerwaarde is dat fouten uit het verhaal zichtbaar worden op de tafel van de
speler. De speler moet een fysieke afbeelding herkennen en de virtuele inhoud
in de echte omgeving herstellen. AR is daarmee nodig om missies te voltooien;
het is meer dan een losse 3D-viewer.

Voor de eerste versie gebruiken we de drie dinokaarten uit workshop 1 als
herkenningskaarten. We koppelen deze aan onze eigen glitchmodellen. We maken de
Image Targets rechtstreeks uit de afbeeldingen met Vuforia's ObserverFactory.
Dit is dezelfde Image Target-techniek, met een andere instelling dan een
vooraf gedownloade Target Manager-database. De trackingkwaliteit moet nog op
een echte telefoon getest worden.

## Welke data gebruikt de app?

- **Glitches:** vaste ID, naam, beschrijving, moeilijkheid, instructie, kaart, XP en aantal stappen.
- **Profiel:** lokale agentnaam. Er is in deze versie één profiel per installatie.
- **Herstellingen:** glitch-ID, voltooiingsdatum en gespeelde tijd.
- **Berekende voortgang:** XP, level, wereldstabiliteit en prestaties.
- **AR-inhoud:** afbeeldingen, 3D-objecten, materialen, colliders en puzzelregels in Unity.

SQLite bewaart het profiel en de herstellingen op een telefoon. In de browser
gebruiken we lokale browseropslag om de Flutter-interface te kunnen testen.
Er is geen server of online account vereist voor de eerste versie.

## Data die AR nodig heeft

Flutter stuurt bij de start de glitch-ID, een nieuwe sessie-ID en de versie van
het berichtcontract. Unity gebruikt de bijbehorende afbeelding, scèneobjecten
en interactieregels. Vuforia heeft cameratoegang en een eigen licentiesleutel
nodig. De sleutel blijft buiten Git.

## Data die AR verandert

Unity stuurt bij voltooiing de glitch-ID, sessie-ID, het aantal opgeloste stappen
en de gespeelde tijd terug. Flutter controleert het bericht en bewaart de
herstelling. De status wordt vervolgens hersteld, de XP stijgt, het level wordt
opnieuw berekend, de wereldstabiliteit stijgt en prestaties worden vrijgespeeld.

Flutter bepaalt de beloning op basis van zijn eigen missiedata. Hetzelfde
missieresultaat of dezelfde missie kan geen dubbele XP opleveren. Annuleren,
trackingverlies of een ongeldige callback levert geen beloning op.

## Haalbaarheid en afbakening

We beperken de eerste versie tot drie kaarten en drie korte puzzels.
Unity gebruikt eenvoudige eigen 3D-vormen, zodat het project niet afhankelijk
is van grote externe modellen. De oefenscène werkt zonder camera in de editor.
De echte camera-ervaring vereist nog een geldige Vuforia-sleutel en
end-to-endtests op Android.

De Flutter-interface is cross-platform opgezet. De eerste Unity-integratie is
op Android gericht. iOS AR is een mogelijke uitbreiding, geen afgerond onderdeel
van deze versie. Ook online accounts en cloudopslag vallen buiten deze versie.
