# Testresultaten en toestelcheck

## Spelupdate van 30 oktober 2026

| Controle | Resultaat |
| --- | --- |
| Flutter-analyse | Geen problemen |
| Flutter-tests | 9 tests geslaagd |
| Unity-puzzelregels en Image Targets | 34 controles geslaagd |
| Nieuwe 3D-previews | Drie beelden gerenderd en visueel gecontroleerd |
| Unity iPhone-framework en Flutter-release | Beide releasebuilds geslaagd |
| Nieuwe spellen op iPhone | Nog te testen; iPhone niet aangesloten |

De Unity-controles gebruiken dezelfde spelklok als de app. Ze controleren de twee
ladingen per kern, verkeerde kernen, gesloten tijdvensters, trackingverlies, de drie
geheugenrondes, verkeerde codes, herhalen, lokvogels, schilden, opnieuw spelen en
precies één voltooiingsbericht. Ook de bestaande XML/DAT-database, de drie From
Database-targets en hun child-puzzels worden gecontroleerd.

De negen Flutter-tests controleren het berichtcontract, oude en dubbele callbacks,
XP zonder dubbele beloning, SQLite-opslag, naamvalidatie, reset, navigatie en
schermen op verschillende breedtes met grote tekst. De spelupdate gebruikt hetzelfde
berichtcontract en dezelfde XP-regels.

De 3D-previews komen uit de Unity-oefenscène. Ze bewijzen de vormgeving, maar niet
de bediening met een vinger of de trackingkwaliteit op een telefoon.

## Eerder op iPhone bevestigd: 29 oktober 2026

Vuforia startte met camera en licentie op de iPhone 15. Alle drie de Image Targets
werden geladen. De stegosauruskaart werd herkend en Mini Black Hole verscheen boven
de kaart. De gebruiker bevestigde dat het spel werkte. Dit was de eerdere spelversie;
de nieuwe timing, schilden en geheugenrondes moeten nog op toestel getest worden.

De eerdere Android-export en debugbuild zijn op 27 oktober gebouwd. Bouw Android
opnieuw om de nieuwe spellen in een APK op te nemen. Een simulator kan echte
Vuforia-tracking niet vervangen.

## Toestelcheck voor de nieuwe spellen

| Test | Verwacht | Status |
| --- | --- | --- |
| Eerste start / AR openen | Dashboard en camera starten | Nog te testen |
| Mini Black Hole | Alleen actieve mintgroene kern telt; twee ladingen per kern | Nog te testen |
| Gravity Error | Lichtcodes van 2, 3 en 4 knoppen; fout herhaalt de huidige ronde | Nog te testen |
| Code herhalen | Huidige ronde opnieuw; afgeronde rondes blijven bewaard | Nog te testen |
| Entity Escaped | Lokvogels tellen niet; gesloten schild blokkeert; drie treffers vangen het wezen | Nog te testen |
| Kaart verdwijnt | Objecten en klok pauzeren; geen tikken verwerkt | Nog te testen |
| Kaart terug | Huidige poging gaat verder | Nog te testen |
| Voltooiing | Korte eindanimatie; juiste missie en XP in Flutter | Nog te testen |
| Opnieuw spelen | Schone puzzel, geen dubbele XP | Nog te testen |
| Geluid | Korte feedback en aparte tonen bij de lichtcode | Nog te testen |
| Achtergrond / terug | Camera en spel hervatten correct | Nog te testen |
| App herstarten | Naam en eerdere herstellingen blijven bewaard | Nog te testen |

Noteer toestel, systeemversie, lichtomstandigheden en de breedte van de getoonde kaart.
