using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectFuelSpawner : MonoBehaviour
{
    public GameObject fuelPrefab;
    public GameObject badFuelPrefab;

    public FuelSpawner correctSpawner;
    public FuelSpawner incorrectSpawner;

    public void FuelBrewed(FuelContent.Recipe recipe)
    {
        if(recipe != null)
        {
            correctSpawner.prefab = fuelPrefab;
            correctSpawner.enabled = true;
            correctSpawner.Spawn();
        }
        else
        {
            incorrectSpawner.prefab = fuelPrefab;
            incorrectSpawner.enabled = true;
            incorrectSpawner.Spawn();
        }
    }
}
