import 'package:flutter/material.dart';
import 'package:reality_exe/app.dart';
import 'unity_mission_page.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  runApp(
    RealityApp(arPageBuilder: (glitch) => UnityMissionPage(glitch: glitch)),
  );
}
