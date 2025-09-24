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
    private LoadingScreen_Controller _loadingScreen;

    private void OnEnable()
    {
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

        // Find the loading screen in bootstrap
        _loadingScreen = FindFirstObjectByType<LoadingScreen_Controller>();
    }

    private void Init(GameManager gameManager)
    {
        GameManager.Instance.SetState(GameState.Gameplay);

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            LoadNextScene();
        }
    }

    #region Public API

    public void Load(SceneReference scene)
    {
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

    public void BeginLoad(SceneReference scene)
    {
        if (_loadingScreen == null)
        {
            Debug.LogError("[SceneLoader] No LoadingScreen_Controller found!");
            return;
        }
        StartCoroutine(BeginLoadRoutine(scene));
    }

    private IEnumerator BeginLoadRoutine(SceneReference scene)
    {
        // Step 1: Fade in (black screen)
        yield return _loadingScreen.FadeRuntime(1f);

        // Step 2: Load scene async
        yield return LoadAsync(scene);

        // Step 3: Fade out happens automatically via OnSceneLoaded event
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

    #endregion Public API
}
