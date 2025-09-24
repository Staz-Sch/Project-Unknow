using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class Bootstrap : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}