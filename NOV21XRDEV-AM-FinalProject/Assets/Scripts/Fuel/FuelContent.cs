using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;





public class FuelContent : MonoBehaviour
{
    [System.Serializable]
    public class Recipe
    {
        public string name;
        public string[] ingredients;

    }

    [System.Serializable]
    public class BrewEvent : UnityEvent<Recipe> { };

    public Recipe[] recipes;
    public BrewEvent OnBrew;

    [Header("Effects")]
    public GameObject electricBuzz;

    private bool canBrew = false;
    public List<string> currentIngredients = new List<string>();


  

    private void OnTriggerEnter(Collider other)
    {
        FuelIngredient ingredient = other.attachedRigidbody.GetComponentInChildren<FuelIngredient>();

        Vector3 contactPosition = other.attachedRigidbody.gameObject.transform.position;
        contactPosition.y = gameObject.transform.position.y;

        var contactEffect = Instantiate(electricBuzz, contactPosition, electricBuzz.transform.rotation);
        Destroy(contactEffect, 1);

        Respawnable respawn = ingredient;
        if(ingredient != null)
        {
            currentIngredients.Add(ingredient.ingredientType);
        }
        else
        {
            currentIngredients.Add("invalid");
            respawn = other.attachedRigidbody.GetComponentInChildren<Respawnable>();
        }
        if(respawn!= null)
        {
            respawn.Respawn();
        }
        else
        {
            Destroy(other.attachedRigidbody.gameObject, 0.5f);
        }
    }

    public void Brew()
    {
        Debug.Log("brewing");
        Recipe recipeBrewed = null;
        foreach(Recipe recipe in recipes)
        {
            List<string> copyOfIngredients = new List<string>(currentIngredients);
            int ingredientCount = 0;

            foreach(var ingredient in recipe.ingredients)
            {
                if (copyOfIngredients.Contains(ingredient))
                {
                    ingredientCount += 1;
                    copyOfIngredients.Remove(ingredient);
                }
            }

            if(ingredientCount == recipe.ingredients.Length)
            {
                recipeBrewed = recipe;
                break;
            }
        }

        ResetFuel();
        StartCoroutine(WaitForBrew(recipeBrewed));
    }

    public void ResetFuel()
    {
        currentIngredients.Clear();
    }

    private IEnumerator WaitForBrew(Recipe recipe)
    {
        //canBrew = false;
        yield return new WaitForSeconds(2f);
        OnBrew.Invoke(recipe);
        //canBrew = true;
    }
}
