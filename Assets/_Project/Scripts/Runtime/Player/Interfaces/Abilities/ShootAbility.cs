using System;
using UnityEngine;

public class ShootAbility : MonoBehaviour, IAbility
{

    [SerializeField] private GameObject _projectile;

    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }




    public void Activate(GameObject Player)
    {
        GameObject ProjectileObj = Instantiate(_projectile);
        Console.WriteLine("Projectile" + ProjectileObj.name);
;


    }
}
