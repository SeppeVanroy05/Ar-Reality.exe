# Code begrijpen, stap voor stap

## 1. Begin bij main.dart

`lib/main.dart` start Flutter en opent `RealityApp`. In `app.dart` wordt de opslag
geopend en de voortgang geladen. Tijdens het laden zie je een laadindicator.
Bij een fout zie je een melding met een knop om opnieuw te proberen.

De versie in `ar_mobile/lib/main.dart` start dezelfde `RealityApp`, maar geeft
ook een functie mee die het Unity-scherm kan maken. Daardoor hoeven we de zes
gewone schermen niet tweemaal te schrijven.

## 2. Wat is een model?

Een model is een klasse met gegevens. `Glitch` beschrijft een missie.
`RepairRecord` beschrijft een voltooide missie. Deze klassen weten niets over
knoppen, kleuren of de camera.

De vaste missies staan in `lib/data/glitches.dart`. Nieuwe beschrijving?
Verander ze daar. Een nieuwe missie toevoegen vraagt ook een Unity-puzzel en kaart.

## 3. Waarom aparte pagina's en widgets?

Elke pagina heeft een eigen bestand. Zo vind je het zoekveld in
`database_page.dart` en het formulier in `profile_page.dart`.

Een `GlitchCard` is een herbruikbaar stukje interface. Het toont een missie en
krijgt een `onTap`-functie mee. Het beslist zelf niet hoe de database werkt.

`StatelessWidget` gebruik je voor weergave zonder eigen veranderlijke waarden.
`StatefulWidget` gebruik je bijvoorbeeld voor een zoekterm, de geselecteerde
filter of een laadstatus. `setState` laat Flutter dat scherm opnieuw tekenen.

## 4. Waar staan de spelregels?

`GameService` bevat gedeelde spelregels. Bijvoorbeeld: een bekende missie mag
maar één keer XP geven. De service erft van `ChangeNotifier`. Dat is een kleine
Flutter-klasse die kan melden dat gegevens zijn veranderd.

`ListenableBuilder` in de hoofdpagina luistert naar die melding. Het dashboard,
profiel en de andere pagina's gebruiken daarna dezelfde bijgewerkte voortgang.
Er is geen extra state-managementpakket nodig.

XP, level en prestaties worden uit de herstellingen berekend. We bewaren geen
losse XP-teller die per ongeluk kan afwijken van de missiestatus.

## 5. Wat doet een repository?

Een repository leest en schrijft gegevens. De schermen schrijven zelf geen SQL.
`SqliteProgressRepository` bewaart herstellingen in de tabel `repairs` en de naam
in `settings`. `BrowserProgressRepository` biedt dezelfde methodes voor web.

`ProgressRepository` is de afspraak die beide klassen volgen: laden, herstelling
bewaren, naam bewaren, voortgang wissen en sluiten. Die kleine abstracte klasse
is hier nuttig omdat er echt twee vormen van opslag zijn.

## 6. Wat betekent async/await?

Opslaan en laden duurt even. Een `Future` stelt het resultaat voor dat later
beschikbaar wordt. Met `await` wacht die functie tot het resultaat er is.
De interface kan intussen een laadindicator tonen.

Daarom worden opslaanknoppen tijdelijk uitgeschakeld. Na een `await` controleert
een pagina met `mounted` of ze nog in beeld mag veranderen. Je voorkomt daarmee
een `setState` op een pagina die de gebruiker al gesloten heeft.

## 7. Hoe loopt één missie door de app?

1. Je kiest Mini Black Hole op de Flutter-detailpagina.
2. De scanner vraagt de Android-runner om het Unity-scherm te openen.
3. Unity meldt dat het klaar is; Flutter stuurt de missie-ID en een nieuwe sessie-ID.
4. Vuforia herkent de kaart. Unity plaatst het zwarte gat op het Image Target.
5. `PointerInput` schiet een onzichtbare straal vanuit de camera door je tikpositie.
6. Een `TapTarget` op de collider geeft de tik door aan `BlackHoleMission`.
7. De puzzel telt drie verschillende energiepunten en meldt voltooiing.
8. `RealityBridge` stuurt een JSON-bericht naar Flutter.
9. `ArBridge` controleert of missie, sessie en aantal stappen kloppen.
10. `GameService` bewaart de herstelling via de repository.
11. De schermen tonen de nieuwe XP, status en prestaties.

JSON is gewoon tekst met benoemde velden, zoals `{"glitchId":"glitch_001"}`.
Een sessie-ID onderscheidt de huidige poging van een oud bericht. Dit is een
controle tegen verkeerde of dubbele callbacks, geen beveiliging tegen cheats.

## 8. Waar vind ik de Unity-code?

- `GlitchMission`: gedeelde start-, stop-, tracking- en voltooiingsregels.
- `BlackHoleMission`: elk energiepunt telt één keer.
- `GravityMission`: controleert de volgorde 3 → 1 → 2.
- `EntityMission`: beweegt het wezen en telt drie treffers.
- `PointerInput`: muis/touch omzetten naar een 3D-raycast.
- `TargetTracking`: trackingstatus doorgeven aan de puzzel.
- `ArBootstrap`: Vuforia starten en Image Targets aanmaken.
- `RealityBridge`: starten vanuit Flutter en resultaten terugsturen.
- `MissionHud`: kleine uitleg en voortgang binnen Unity.

De editorbestanden onder `Assets/Reality/Editor` zijn hulpmiddelen voor het
maken en controleren van scènes. Je hoeft die niet eerst te begrijpen om de
puzzels te wijzigen. De scènes staan al klaar en zijn gewoon bewerkbaar.

## 9. Bewuste keuzes

We gebruiken gewone klassen, `setState`, een kleine service en repositories.
Geen Riverpod, BLoC, codegenerator, backend of login voor deze offline versie.
Er zijn twee Flutter-startpunten, maar er is maar één implementatie van de UI.
Dit voorkomt dat een ontbrekende Unity-export het werken aan Flutter blokkeert.

De Dart-bestandsnamen en klassen zijn Engels, zoals in de cursus en Flutter.
De uitleg, foutmeldingen en opmerkingen zijn Nederlands.
