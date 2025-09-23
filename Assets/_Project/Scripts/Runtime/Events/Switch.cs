using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class BoolEvent : UnityEvent<bool> { };

public class Switch : MonoBehaviour
{
    public BoolEvent OnSwitchToggled;
    [SerializeField]private bool isOn = false;


    [SerializeField]private bool playerInRange = false;


    private void Update()
    {
        if (playerInRange&& Input.GetKeyUp(KeyCode.E))
        {
            isOn = !isOn;
            Debug.Log($"Switch is now {(isOn ? "ON" : "OFF")}");
            OnSwitchToggled?.Invoke(isOn);
        }   
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        playerInRange = false;
    }
}