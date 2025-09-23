using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> Prefabs = new List<GameObject> ();
    public float force = 10f;
    public Vector3 gravity = Physics.gravity;

    public float muiltyplayer;

    private List<GameObject> _spawnedObjects= new List<GameObject> ();

    public ForceMode forceMode;

    private Camera Camera;

    private void Start()
    {
        Camera = Camera.main;
    }


    private void Update()
    {
        if (Prefabs.Count == 0) Debug.Log("Prefabs list is empty");
        else if (Prefabs.Count != 0 && Input.GetMouseButtonUp(0))
        {
            GameObject rand = Prefabs[Random.Range(0, Prefabs.Count)];
            GameObject clone = Instantiate(rand, Camera.transform.position, Camera.transform.localRotation);
            _spawnedObjects.Add(clone);

            Rigidbody cloneRB = clone.GetComponent<Rigidbody>();
            cloneRB.AddForce(clone.transform.forward*force, forceMode);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach (GameObject obj in _spawnedObjects)
            {
                Destroy(obj);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Physics.gravity = new Vector3(0,-gravity.y*muiltyplayer, 0);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            Physics.gravity = gravity;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Physics.gravity = new Vector3(-muiltyplayer, gravity.y, 0);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            Physics.gravity = gravity;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Physics.gravity = new Vector3(muiltyplayer, gravity.y, 0);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            Physics.gravity = gravity;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Physics.gravity = new Vector3(0, gravity.y, muiltyplayer);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            Physics.gravity = gravity;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Physics.gravity = new Vector3(0, gravity.y, -muiltyplayer);
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            Physics.gravity = gravity;
        }



    }
}
