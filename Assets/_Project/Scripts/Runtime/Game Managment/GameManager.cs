using Sirenix.OdinInspector;
using System;
using UnityEngine;

[RequireComponent(typeof(SceneLoader))]
public class GameManager : MonoBehaviour
{
    [ShowInInspector] public GameState CurrentState { get; private set; }

    public static event Action<GameState> OnGameStateChanged;

    public static event Action<GameManager> OnAlive;

    public static bool IsAlive { get; private set; }

    #region Singleton

    public static GameManager Instance { get; private set; }

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

    private void Start()
    {
        IsAlive = true;
        OnAlive?.Invoke(Instance);
        Debug.Log("[GM] Broadcasting OnAlive with " + Instance.name);
    }

    #endregion Singleton

    #region Helpers

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    #endregion Helpers
}