using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelSpawner : MonoBehaviour
{
    public GameObject prefab;
    public Transform spawnPoint;
    public int maxInstances = 2;
    public int minInstances = 0;

    List<GameObject> instances = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        for(int i = 0; i< instances.Count; ++i)
        {
            if(instances[i] == null)
            {
                instances.RemoveAt(i);
                i--;
            }
        }
        if(prefab != null)
        {
            while(instances.Count< minInstances)
            {
                Spawn();
            }
        }
    }

    public void Spawn()
    {
        var newFuel = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        if(instances.Count >= maxInstances)
        {
            Destroy(instances[0]);
            instances.RemoveAt(0);
        }

        instances.Add(newFuel);
    }
}
