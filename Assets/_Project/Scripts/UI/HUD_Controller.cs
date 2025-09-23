using UnityEngine;
using TMPro;

public class HUD_Controller : MonoBehaviour
{
    public TextMeshProUGUI test;


    private void Awake()
    {
        if (test == null)
        {
            Debug.Log("tmp is null");
        }
    }




}
