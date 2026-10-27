# Testresultaten en toestelcheck

Gecontroleerd op 27 oktober 2026.

## Uitgevoerde controles

| Controle | Resultaat |
| --- | --- |
| Flutter `flutter analyze` | Geen problemen |
| Android-runner `flutter analyze` | Geen problemen |
| `flutter test` | 9 tests geslaagd |
| Flutter web releasebuild | Geslaagd |
| Unity C#-compilatie met Vuforia 11.4.4 | Geslaagd |
| Unity-puzzelcontroles | 10 controles geslaagd |
| Unity-export voor Android/Flutter | Geslaagd |
| Android debug-APK met Unity | Gebouwd |
| Flutter-browserbeelden op telefoonformaat | Visueel bekeken |
| Drie Unity-3D-previews | Gerenderd en bekeken |

De Flutter-tests controleren het berichtcontract, verkeerde en dubbele callbacks,
XP zonder dubbele beloning, de SQLite-tabellen, opnieuw laden van voortgang,
naamvalidatie, reset, navigatie, zoeken en layouts op 320 en 1200 pixels met
vergrote tekst. De SQLite-tests gebruiken een echte SQLite-engine in geheugen;
het herstarten van een geïnstalleerde app moet nog op toestel getest worden.

De Unity-controles proberen de puzzelregels rechtstreeks. Ze controleren onder
meer dubbele energietikken, trackingverlies, opnieuw spelen, de codevolgorde,
het vangen van het wezen en de juiste exportscène. Ze bewijzen nog niet dat
cameratracking, touchbediening en native callbacks op een telefoon werken.

De 3D-previews zijn beelden uit de Unity-camera van de oefenscène. Het zijn geen
AR-foto's van een telefoon. De APK bevat nog geen eigen Vuforia-sleutel en is
met een debugcertificaat ondertekend. Hij is bedoeld voor ontwikkeling.

## Nog uit te voeren op een echte Android-telefoon

Vul deze tabel samen aan na het toevoegen van jullie eigen Vuforia-sleutel
en een nieuwe Unity-export.

| Test | Verwacht | Resultaat |
| --- | --- | --- |
| Eerste start | Dashboard laadt zonder fout | Nog te testen |
| Camera weigeren | Duidelijke melding, geen XP | Nog te testen |
| Camera toestaan | Unity-camerabeeld verschijnt | Nog te testen |
| Juiste kaart scannen | Bijbehorende 3D-puzzel verschijnt | Nog te testen |
| Verkeerde kaart tonen | Gekozen missie kan niet gespeeld worden | Nog te testen |
| Kaart uit beeld | Object verdwijnt; tikken tellen niet | Nog te testen |
| Kaart terug in beeld | Zelfde poging kan verdergaan | Nog te testen |
| Mini Black Hole | Drie verschillende punten herstellen de glitch | Nog te testen |
| Gravity Error | 3 → 1 → 2 slaagt, fout reset de reeks | Nog te testen |
| Entity Escaped | Wezen beweegt, drie treffers sluiten missie af | Nog te testen |
| Geslaagde missie | Flutter toont juiste status en XP | Nog te testen |
| App volledig herstarten | Naam en herstellingen blijven bewaard | Nog te testen |
| Missie opnieuw spelen | Geen dubbele XP | Nog te testen |
| Terug tijdens missie | Geen beloning | Nog te testen |
| App naar achtergrond en terug | Unity hervat correct | Nog te testen |
| Tweede missie starten | Nieuwe sessie, juiste puzzel, geen oud resultaat | Nog te testen |
| Voortgang wissen | Eerst bevestiging, daarna lege voortgang | Nog te testen |

Noteer ook toestel, Android-versie, lichtomstandigheden, kaartbreedte en
de gebruikte Unity-/Flutter-versies.
