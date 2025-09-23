using Sirenix.OdinInspector;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneReferences", menuName = "Scene References/New Scene Reference")]
public class SceneReference : ScriptableObject
{
    public SceneAsset sceneAsset;
    [ShowInInspector, ReadOnly] private string scenePath;
    public GameState DefaultState;

    private void OnValidate()
    {
        var path = AssetDatabase.GetAssetPath(sceneAsset);
        scenePath = Path.GetFileNameWithoutExtension(path);
    }

    public string GetScenePath()
    {
        return scenePath;
    }
}