#!/bin/bash
# Bouw de echte Unity-scene voor iPhone en leg het framework klaar voor Flutter.
set -euo pipefail
PROJECT_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
REALITY_UNITY_EDITOR="${REALITY_UNITY_EDITOR:-/Applications/Unity/Hub/Editor/6000.3.24f1/Unity.app/Contents/MacOS/Unity}"
"$REALITY_UNITY_EDITOR" -batchmode -nographics -quit -buildTarget iOS   -projectPath "$PROJECT_ROOT/unity/RealityUnity"   -executeMethod VerifyRealityProject.PrepareIosExport   -exportPath "$PROJECT_ROOT/ar_mobile/ios/unityLibrary"
xcodebuild -project "$PROJECT_ROOT/ar_mobile/ios/unityLibrary/Unity-iPhone.xcodeproj"   -target UnityFramework -configuration Release -sdk iphoneos -arch arm64   "CONFIGURATION_BUILD_DIR=$PROJECT_ROOT/ar_mobile/build/unity-ios"   CODE_SIGNING_ALLOWED=NO IPHONEOS_DEPLOYMENT_TARGET=15.0
ditto "$PROJECT_ROOT/ar_mobile/build/unity-ios/UnityFramework.framework"   "$PROJECT_ROOT/ar_mobile/ios/UnityFramework.framework"
for framework in VuforiaEngine UnityDriver; do
  framework_path="$(find "$PROJECT_ROOT/ar_mobile/ios/unityLibrary/Frameworks" -name "$framework.framework" -type d -print -quit)"
  test -n "$framework_path"
  ditto "$framework_path" "$PROJECT_ROOT/ar_mobile/ios/$framework.framework"
done
echo "Unity voor iPhone is klaar. Start nu Flutter vanuit ar_mobile."
