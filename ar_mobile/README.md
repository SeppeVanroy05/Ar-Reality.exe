# Mobiele app met echte Unity AR

Dit is een dunne mobiele runner: alle gewone schermen komen uit `../lib`.
Alle 3D-objecten, cameratracking en puzzels komen uit `../unity/RealityUnity`.

Voor Android bouwt deze runner pas nadat je Unity hebt geëxporteerd naar
`android/unityLibrary`. Volg eerst `../docs/UNITY_STARTEN.md`.

Daarna:

```sh
flutter pub get
flutter run -d <android-device-id>
```

Gebruik `flutter devices` om het ID te vinden. Een echt Android-toestel met
cameratoegang is nodig om Vuforia te testen. Voor iPhone: volg `../docs/IPHONE_STARTEN.md` en bouw eerst het Unity-framework.
