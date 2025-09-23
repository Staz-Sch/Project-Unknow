using UnityEngine.SceneManagement;

public static class GameSettings
{
    public static string BootScene = SceneManager.GetSceneByBuildIndex(0).name;
}