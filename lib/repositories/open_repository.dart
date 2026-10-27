import 'package:flutter/foundation.dart';
import 'browser_progress_repository.dart';
import 'sqlite_progress_repository.dart';
import 'progress_repository.dart';

Future<ProgressRepository> openRepository() async {
  if (kIsWeb) return BrowserProgressRepository.open();
  return SqliteProgressRepository.open();
}
