using UnityEngine;

public class MainMenu_UI : MonoBehaviour
{
    [SerializeField] private SceneReference gameplayScene;

    public void OnStartPressed()
    {
        SceneLoader.Instance.BeginLoad(gameplayScene);
    }

    public void OnQuitPressed()
    {
        SceneLoader.Instance.QuitGame();
    }
}
