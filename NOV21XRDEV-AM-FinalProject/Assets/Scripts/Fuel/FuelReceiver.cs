using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class FuelPouredEvent: UnityEvent<string> { }

public class FuelReceiver : MonoBehaviour
{
    private bool correctPoured = false;
    public string[] acceptedFuelType;
    public FuelPouredEvent OnFuelPoured;

    public void RecieveFuel(string fuelType)
    {
        if(acceptedFuelType.Contains(fuelType) && !correctPoured)
        {
            OnFuelPoured.Invoke(fuelType);
            correctPoured = true;
        }
    }

}
