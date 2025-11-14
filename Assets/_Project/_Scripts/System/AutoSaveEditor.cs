#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class AutoSaveEditor
{
    private static double _lastSaveTime;
    private static double _saveInterval = 300; // Save every 5 minutes (in seconds)

    static AutoSaveEditor()
    {
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
        {
            return; // Don't save during play mode or compilation
        }

        if (EditorApplication.timeSinceStartup > _lastSaveTime + _saveInterval)
        {
            Debug.Log("Auto-saving scenes and assets...");
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            _lastSaveTime = EditorApplication.timeSinceStartup;
        }
    }
}
#endif