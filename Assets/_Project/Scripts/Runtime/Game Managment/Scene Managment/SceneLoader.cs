using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    // Events
    public static event Action<SceneReference> OnBeforeSceneUnload;
    public static event Action<SceneReference, float> OnSceneLoadProgress;
    public static event Action<SceneReference> OnSceneLoaded;

    private SceneReference _currentScene;

    [SerializeField]private GameManager _gameManager;

     private void OnEnable()
    {
        Debug.Log("[SL] OnEnable, IsAlive = " + GameManager.IsAlive);
        GameManager.OnAlive += Init;
        if (GameManager.IsAlive && GameManager.Instance != null)
        {
            Init(GameManager.Instance);
        }
    }

    private void OnDisable()
    {
        GameManager.OnAlive -= Init;
    }

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Init(GameManager gameManager)
    {
        _gameManager = gameManager;
        Debug.Log($"Scene Loader ran Init with {gameManager.name}");

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            LoadNextScene();
        }
    }

    #region Public API

    public void Load(SceneReference scene)
    {
        Debug.Log("[SL] Load called, _gameManager = " + GameManager.Instance);

        var path = scene.GetScenePath();
        var newState = scene.DefaultState;

        if (!string.IsNullOrEmpty(path))
        {
            OnBeforeSceneUnload?.Invoke(scene);
            SceneManager.LoadScene(path);
            _currentScene = scene;
            GameManager.Instance.SetState(newState);
            OnSceneLoaded?.Invoke(_currentScene);
        }
        else
        {
            Debug.LogWarning($"[SceneController] No Path found in {scene.name}");
        }
    }

    public IEnumerator LoadAsync(SceneReference scene)
    {
        var path = scene.GetScenePath();
        if (string.IsNullOrEmpty(path)) yield break;

        OnBeforeSceneUnload?.Invoke(_currentScene);

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(path);
        while (!asyncOp.isDone)
        {
            OnSceneLoadProgress?.Invoke(scene, asyncOp.progress);
            yield return null;
        }

        _currentScene = scene;
        OnSceneLoaded?.Invoke(scene);
    }

    public void ReloadCurrentScene()
    {
        var name = SceneManager.GetActiveScene().name;
        if (!string.IsNullOrEmpty(name))
        {
            SceneManager.LoadScene(name);
        }
    }

    public void LoadNextScene()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion
}
