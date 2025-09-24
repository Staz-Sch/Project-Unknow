using UnityEngine;

public class SceneLoaderDebugListener : MonoBehaviour
{
    private void OnEnable()
    {
        SceneLoader.OnBeforeSceneUnload += HandleBeforeUnload;
        SceneLoader.OnSceneLoadProgress += HandleProgress;
        SceneLoader.OnSceneLoaded += HandleLoaded;
    }

    private void OnDisable()
    {
        SceneLoader.OnBeforeSceneUnload -= HandleBeforeUnload;
        SceneLoader.OnSceneLoadProgress -= HandleProgress;
        SceneLoader.OnSceneLoaded -= HandleLoaded;
    }

    private void HandleBeforeUnload(SceneReference scene)
    {
        Debug.Log($"[DebugListener] Before unloading scene: {scene?.name}");
    }

    private void HandleProgress(SceneReference scene, float progress)
    {
        Debug.Log($"[DebugListener] Loading {scene.name}: {progress * 100f}%");
    }

    private void HandleLoaded(SceneReference scene)
    {
        Debug.Log($"[DebugListener] Finished loading scene: {scene.name}");
    }
}