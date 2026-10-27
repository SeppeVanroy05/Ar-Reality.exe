# Samenwerken met Git

De buitenste projectmap is de Git-repository. Flutter en Unity staan samen in
één repository. Unity Library, exports, builds, licentiesleutels en lokale
IDE-bestanden worden genegeerd. Unity `.meta`-bestanden worden juist wel bijgehouden:
daarmee bewaart Unity de verwijzingen tussen scènes en scripts.

## GitHub-repository

De code staat op [SeppeVanroy05/Ar-Reality.exe](https://github.com/SeppeVanroy05/Ar-Reality.exe),
op branch `main`. De lokale remote `origin` gebruikt SSH en is gekoppeld aan
`git@github.com:SeppeVanroy05/Ar-Reality.exe.git`.

De tweede student kan het project ophalen met:

```sh
git clone https://github.com/SeppeVanroy05/Ar-Reality.exe.git
cd Ar-Reality.exe
```

Na een clone moet Unity packages opnieuw importeren. Voor Android moet iedere
student lokaal de Vuforia-sleutel toevoegen en de Unity-export maken. De
licentiesleutel en gegenereerde Unity-export worden niet naar GitHub gepusht.

## Werk per wijziging op een branch

```sh
git switch main
git pull
git switch -c feature/mijn-wijziging
```

Test je wijziging. Bekijk dan wat je gaat toevoegen:

```sh
git status
git diff
git add <aangepaste-bestanden>
git commit -m "Beschrijf kort wat er verandert"
git push -u origin feature/mijn-wijziging
```

Maak op GitHub een pull request en laat je teamgenoot meekijken. Spreek af wie
welke Unity-scène wijzigt: tegelijk dezelfde scène aanpassen kan conflicten geven.

## Voorstel taakverdeling

- Student 1: Flutter-schermen, lokale opslag, profiel en voortgang.
- Student 2: Unity-puzzels, Vuforia, 3D-interacties en AR-toesteltests.
- Samen: berichtcontract, integratie, onderzoek en examenpresentatie.

Iedereen moet uiteindelijk beide delen kunnen uitleggen.
