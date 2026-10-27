# Koppeling met de cursus en onderzoek

Bron: https://itf-mobiledevelopment.netlify.app/flutter/ (bekeken op 27 oktober 2026).
Ook de Flutter-integratiepagina onder Vuforia en de aangeleverde Unity-workshops
zijn bekeken. De MAUI/Azure-bestanden in Archive(1) gaan over andere technologie
en zijn niet als Flutter-architectuur overgenomen.

## Flutter-hoofdstukken

| Onderdeel uit de cursus | Toepassing |
| --- | --- |
| Introduction / Get started | Gewoon Flutter-project; cursusversie 3.44.0 |
| Creating your first App | main, MaterialApp, Scaffold en aparte paginaklassen |
| Designing the UI with Widgets | Row, Column, Expanded, Padding, Card en herbruikbare widgets |
| Building a Stateful App | setState, modellen, dropdown en TextEditingController |
| Firebase project and Firestore | Onderzocht; bewust lokale opslag volgens jullie voorstel |
| Firebase Authentication | Onderzocht; geen online account nodig voor de lokale MVP |
| Firestore data in a ListView | Repository-scheiding toegepast; lokale lijst in plaats van Firestore-stream |
| Forms and validation | Profielnaam met Form, GlobalKey, validator en opslagfeedback |
| Firestore security rules | Niet van toepassing zonder Firebase; nodig bij latere cloudversie |
| Architecture and state management | models/pages/repositories/services, ChangeNotifier voor gedeelde voortgang |
| Material 3 design | Eén centraal thema, ColorScheme en standaard Material-widgets |
| Responsive and adaptive UI | SafeArea, beperkte inhoudsbreedte, NavigationBar en NavigationRail |
| Accessibility | Labels, tekst bij status, schaalbare layout en schermtests met grote tekst |
| Debugging | flutter analyze, gerichte tests en concrete foutmeldingen |
| Workshop 1: Match Maker | Kleine widgets, heldere mappenstructuur en scrollbare inhoud |

De Firebase-hoofdstukken behandelen een online voorbeeldapp. Dat betekent niet
dat Firebase nodig is voor elke app. Dit project volgt jullie keuze voor lokale
opslag. Als de docent Firebase afzonderlijk verplicht stelt, moet die uitbreiding
nog toegevoegd worden; we doen niet alsof login of cloudopslag al aanwezig is.

## Unity-workshops

Workshop 1 gebruikt Vuforia Image Targets, 3D-inhoud, MonoBehaviour, Start/Update
en een Android-build met IL2CPP en ARM64. Die basis past bij REALITY.EXE.
Workshop 3-1 behandelt onder meer raycasting, colliders en beweging met sinus en
cosinus. Die technieken gebruiken we voor tikken op objecten en het bewegende wezen.

Cylinder Targets en Model Targets uit workshop 3 zijn onderzocht, maar niet
gekozen: onze missies zijn gekoppeld aan vlakke kaarten. Model Targets vragen
bijkomende objecten en voorbereiding zonder duidelijke meerwaarde voor deze MVP.

## Vergelijking AR flavors

| Optie | Past bij dit idee? | Beslissing |
| --- | --- | --- |
| Image Targets | Ja: goedkoop af te drukken, voorspelbaar anker per missie | Gekozen |
| Cylinder Targets | Alleen zinvol met bijvoorbeeld blikken of flessen | Niet nodig voor vlakke kaarten |
| Model Targets | Interessant voor herkenning van een echt 3D-object | Buiten deze MVP |
| Plaatsing op een vlak zonder marker | Interessant, maar verandert de opdracht en opzet | Mogelijke latere uitbreiding |

Dit zijn ontwerpkeuzes voor dit project, geen algemene rangschikking van AR-technieken.

## Onderzochte technische risico's

| Vraag | Huidige bevinding | Nog te controleren |
| --- | --- | --- |
| Compileert de Unity-code? | Ja, met Unity 6000.3.24f1 en Vuforia 11.4.4 | Nieuwe wijzigingen opnieuw controleren |
| Werken de puzzelregels? | Tien geautomatiseerde controles geslaagd | Volledige bediening in Play Mode |
| Kan Unity voor Flutter exporteren? | Android-export geslaagd met Flutter Embed | Cameraketen op echt toestel |
| Hoe werkt de communicatie? | JSON met versie, missie en unieke sessie; Dart-contract getest | Callback van native Unity op toestel |
| Hoe bewaren we voortgang? | SQLite met unieke missie-ID; tests slagen | Herstart van geïnstalleerde mobiele app |
| Wat gebeurt bij trackingverlies? | Unity blokkeert tikken en verbergt 3D-inhoud | Verlies en herdetectie met echte kaart |
| Herkent Vuforia de leskaarten voldoende? | Rechtstreekse Image Targets aangemaakt | Licht, hoek, afstand en kaartkwaliteit op toestel |

## Bronnen

- Cursus: https://itf-mobiledevelopment.netlify.app/flutter/
- Architectuur: https://itf-mobiledevelopment.netlify.app/flutter/architecture-state-management
- Flutter/Unity-cursus: https://itf-mobiledevelopment.netlify.app/vuforia/flutter-integration
- Embed Unity 2.0.0: https://pub.dev/packages/flutter_embed_unity/versions/2.0.0
- Officieel pluginvoorbeeld: https://github.com/learntoflutter/flutter_embed_unity
- Vuforia-package uit de les: https://github.com/mcloots/vuforia_package_11.4.4
- Vuforia runtime-targets: https://developer.vuforia.com/library/vuforia-engine/images-and-objects/image-targets/instant-image-targets/
- SQLite-plugin: https://pub.dev/packages/sqflite
- Browseropslag: https://pub.dev/packages/shared_preferences

De pluginversies en de Vuforia Git-revisie zijn vastgelegd in de projectbestanden.
