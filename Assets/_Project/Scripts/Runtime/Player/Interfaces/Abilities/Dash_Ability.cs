using UnityEngine;

public class Dash_Ability : MonoBehaviour, IAbility
{
    [SerializeField] private float _dashMultiplyer;

    private CharacterController _playerCC;

    public void Activate(GameObject Player)
    {
        _playerCC = Player.GetComponent<CharacterController>();
        _playerCC.attachedRigidbody.AddForce(Player.transform.forward * _dashMultiplyer, ForceMode.Impulse);
    }
}
