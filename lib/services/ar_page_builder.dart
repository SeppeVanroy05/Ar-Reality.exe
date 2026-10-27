import 'package:flutter/widgets.dart';
import '../models/glitch.dart';

// De gewone Flutter-app kent de Unity-plugin niet. De mobiele runner levert dit scherm.
typedef ArPageBuilder = Widget Function(Glitch glitch);
