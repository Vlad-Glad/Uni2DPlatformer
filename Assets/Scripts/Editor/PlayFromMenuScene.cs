using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class PlayFromMenuScene
{
    static PlayFromMenuScene()
    {
        EditorSceneManager.playModeStartScene =
            AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Menu.unity");
    }
}