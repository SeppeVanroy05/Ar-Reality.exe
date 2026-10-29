# Lokale compatibiliteitsaanpassing

Bron: flutter_embed_unity_2022_3_ios 2.0.0 van pub.dev (MIT; zie LICENSE).
De statische UnityFramework-stubs krijgen een module.modulemap in hun Headers-map,
zodat Swift Package Manager de Objective-C-interface als module kan importeren.
De echte UnityFramework.framework wordt afzonderlijk uit dit Unity-project gebouwd
en door Runner ingebed. De eigenlijke pluginlogica is ongewijzigd.

Ook de meegeleverde UnityAppController.h-stub wordt naast de statische headers
geplaatst. De import gebruikt een relatieve headernaam, omdat dit een statische
bibliotheek is en geen framework-bundle. Er is geen nieuwe Unity-implementatie toegevoegd.
