using UnityEngine;


namespace StarterKit
{
    [RequireComponent(SceneLoader)]
    [DefaultExecutionOrder(-1000)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public GameState State { get; private set; }


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Init();
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        { if (Instance == this) Instance = null; }

        private void Init()
        {
            SetGameState(GameState.StartMenu);

        }

        public void SetGameState(GameState s)
        {
            State = s;
        }
    }
}