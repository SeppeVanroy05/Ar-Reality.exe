"""Voer uit na elke Unity-export. Alleen gegenereerde bestanden worden aangepast."""
from pathlib import Path
import re

root = Path(__file__).resolve().parents[1]
export = root / 'ar_mobile/android/unityLibrary'
if not (export / 'build.gradle').exists():
    raise SystemExit('Unity-export ontbreekt. Exporteer eerst via Flutter Embed > Android.')

gradle = export / 'build.gradle'
gradle.write_text(re.sub(r'ndkVersion\s+"[^"]+"',
    'ndkVersion "28.2.13676358"', gradle.read_text()))
proguard = export / 'proguard-unity.txt'
if proguard.exists():
    proguard.write_text(re.sub(r'^\s*-ignorewarnings\s*$',
        '# -ignorewarnings verwijderd volgens de cursus', proguard.read_text(), flags=re.MULTILINE))
print('Unity-export voorbereid: NDK 28.2.13676358, ProGuard gecontroleerd.')
