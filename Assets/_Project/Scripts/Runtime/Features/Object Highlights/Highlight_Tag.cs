using UnityEngine;

public class Highlight_Tag : MonoBehaviour
{
    [HideInInspector] public int originalLayer;

    private void Awake()
    {
        originalLayer = gameObject.layer;
    }


}
