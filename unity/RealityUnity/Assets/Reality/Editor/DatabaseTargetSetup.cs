using System.IO;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using Vuforia;

// Controleer de echte databasekoppelingen voordat een Android-export wordt gemaakt.
public class DatabaseTargetSetup : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public static void ImportDatabase()
    {
        AssetDatabase.importPackageCompleted += OnDatabaseImported;
        AssetDatabase.importPackageFailed += OnImportFailed;
        string package = Path.GetFullPath("../Database/REALITY_EXE.unitypackage");
        AssetDatabase.ImportPackage(package, false);
    }

    private static void OnImportFailed(string package, string error)
    {
        Debug.LogError("Database-import mislukt: " + error);
        if (Application.isBatchMode) EditorApplication.Exit(1);
    }

    private static void OnDatabaseImported(string package)
    {
        AssetDatabase.importPackageCompleted -= OnDatabaseImported;
        AssetDatabase.Refresh();
        ConfigureDatabase();
        Debug.Log("REALITY database-import en koppelingen voltooid.");
        if (Application.isBatchMode) EditorApplication.Exit(0);
    }

    [MenuItem("REALITY/5 - Koppel REALITY_EXE-database")]
    public static void ConfigureDatabase()
    {
        EditorSceneManager.OpenScene("Assets/Reality/Scenes/RealityAR.unity");
        var bootstrap = Object.FindFirstObjectByType<ArBootstrap>();
        foreach (var target in bootstrap.imageTargets)
        {
            string targetName = target.GetComponent<TargetTracking>().mission.glitchId;
            var xml = XDocument.Load("Assets/StreamingAssets/Vuforia/REALITY_EXE.xml");
            var entry = xml.Descendants("ImageTarget").Single(x => (string)x.Attribute("name") == targetName);
            string[] size = ((string)entry.Attribute("size")).Split(' ');
            float width = float.Parse(size[0], CultureInfo.InvariantCulture);
            float height = float.Parse(size[1], CultureInfo.InvariantCulture);
            var settings = new SerializedObject(target);
            settings.FindProperty("mImageTargetType").intValue = (int)ImageTargetType.PREDEFINED;
            settings.FindProperty("mDataSetPath").stringValue = "Assets/StreamingAssets/Vuforia/REALITY_EXE.xml";
            settings.FindProperty("mTrackableName").stringValue = targetName;
            settings.FindProperty("mInitializedInEditor").boolValue = true;
            settings.FindProperty("mWidth").floatValue = width;
            settings.FindProperty("mHeight").floatValue = height;
            settings.FindProperty("mAspectRatio").floatValue = height / width;
            settings.ApplyModifiedPropertiesWithoutUndo();
            target.SetWidth(width);
            if (Mathf.Abs(target.GetSize().y - height) > 0.00001f)
                throw new BuildFailedException("Targetverhouding wijkt af van de gedownloade database.");
            EditorUtility.SetDirty(target);
            Debug.Log("REALITY target gekoppeld: " + target.TargetName + " size=" + target.GetSize());
        }
        RequireConfiguredTargets();
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
    }

    [MenuItem("REALITY/4 - Controleer Image Target-database")]
    public static void ValidateScene()
    {
        EditorSceneManager.OpenScene("Assets/Reality/Scenes/RealityAR.unity");
        RequireConfiguredTargets();
        Debug.Log("REALITY: alle drie de Image Targets zijn gekoppeld aan een bestaande database.");
    }

    public static void RequireConfiguredTargets()
    {
        var bootstrap = Object.FindFirstObjectByType<ArBootstrap>();
        if (bootstrap == null || bootstrap.imageTargets == null || bootstrap.imageTargets.Length != 3)
            throw new BuildFailedException("RealityAR moet drie Image Targets bevatten.");
        foreach (var target in bootstrap.imageTargets)
        {
            if (target == null)
                throw new BuildFailedException("Een Image Target-verwijzing ontbreekt op RealityBridge.");
            var settings = new SerializedObject(target);
            string database = settings.FindProperty("mDataSetPath").stringValue;
            string name = settings.FindProperty("mTrackableName").stringValue;
            if (target.ImageTargetType != ImageTargetType.PREDEFINED ||
                string.IsNullOrWhiteSpace(database) || string.IsNullOrWhiteSpace(name))
                throw new BuildFailedException(target.name + ": importeer de Vuforia-database en kies " +
                    "Type: From Database, Database en Image Target in de Inspector. Zie docs/UNITY_STARTEN.md.");
            // Vuforia kan het pad als projectpad of relatief aan StreamingAssets bewaren.
            string fullPath = Path.IsPathRooted(database) || database.StartsWith("Assets/")
                ? database : Path.Combine(Application.streamingAssetsPath, database);
            if (!File.Exists(fullPath) || !File.Exists(Path.ChangeExtension(fullPath, ".dat")))
                throw new BuildFailedException(target.name + ": het XML- of DAT-bestand van de database ontbreekt.");
            var tracking = target.GetComponent<TargetTracking>();
            if (tracking == null || tracking.mission == null || tracking.mission.transform.parent != target.transform)
                throw new BuildFailedException(target.name + ": de puzzel moet een child van het Image Target zijn.");
        }
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        // De exportfunctie opent RealityAR eerst. Ook handmatig bouwen moet de juiste scene gebruiken.
        RequireConfiguredTargets();
    }
}
