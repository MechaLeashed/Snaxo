using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlatingStation : MonoBehaviour, IInteractable
{
    [Header("Plating Configuration")]
    public Transform plateTransform; // Where ingredients are placed visually
    public List<RecipeData> availableRecipes;

    private List<IngredientData> currentIngredients = new List<IngredientData>();
    private List<GameObject> ingredientVisuals = new List<GameObject>();

    private IngredientData finishedMealData;
    private GameObject finishedMealVisual;

    public void Interact(PlayerController player)
    {
        // If the player is holding something, try to add it to the plate
        if (player.IsHoldingItem())
        {
            if (finishedMealData != null)
            {
                Debug.Log("Plate is already full with a finished meal!");
                return;
            }

            IngredientData heldItem = player.GetHeldItem();

            // Add ingredient data
            currentIngredients.Add(heldItem);

            // Take the physical object from the player
            GameObject visual = player.GetCurrentObject();
            visual.transform.SetParent(plateTransform);

            // Stack ingredients slightly on top of each other
            visual.transform.localPosition = new Vector3(0, currentIngredients.Count * 0.1f, 0);
            visual.transform.localRotation = Quaternion.identity;

            ingredientVisuals.Add(visual);
            player.ClearHand();

            // Check if current ingredients match any recipe
            CheckRecipes();
        }
        else
        {
            // If the player is empty-handed, pick up what is on the plate
            if (finishedMealData != null)
            {
                // Pick up the finished meal
                player.PickUpItem(finishedMealVisual, finishedMealData);
                finishedMealData = null;
                finishedMealVisual = null;
            }
            else if (currentIngredients.Count > 0)
            {
                // Pick up the last added ingredient
                int lastIdx = currentIngredients.Count - 1;
                player.PickUpItem(ingredientVisuals[lastIdx], currentIngredients[lastIdx]);

                currentIngredients.RemoveAt(lastIdx);
                ingredientVisuals.RemoveAt(lastIdx);
            }
        }
    }

    private void CheckRecipes()
    {
        foreach (RecipeData recipe in availableRecipes)
        {
            if (HasAllIngredients(recipe.requiredIngredients, currentIngredients))
            {
                FinishPlating(recipe);
                break; // Stop checking once a recipe is matched
            }
        }
    }

    private bool HasAllIngredients(List<IngredientData> required, List<IngredientData> current)
    {
        if (required.Count != current.Count) return false;

        // Compare names to check if the ingredients match regardless of order
        var reqNames = required.Select(i => i.ingredientName).OrderBy(n => n).ToList();
        var curNames = current.Select(i => i.ingredientName).OrderBy(n => n).ToList();

        return reqNames.SequenceEqual(curNames);
    }

    private void FinishPlating(RecipeData recipe)
    {
        // 1. Remove raw ingredient models
        foreach (var vis in ingredientVisuals)
        {
            Destroy(vis);
        }
        ingredientVisuals.Clear();
        currentIngredients.Clear();

        // 2. Set the finished meal data
        finishedMealData = recipe.finishedMeal;

        // 3. Instantiate the finished meal model on the plate
        if (finishedMealData.prefab != null)
        {
            finishedMealVisual = Instantiate(finishedMealData.prefab, plateTransform);
            finishedMealVisual.transform.localPosition = Vector3.zero;
        }
        else
        {
            // FALLBACK: If the user forgot to assign a prefab in the IngredientData, spawn a temporary cube so it doesn't crash!
            Debug.LogError($"Missing Prefab on {finishedMealData.ingredientName}! Spawning a temporary cube.");
            finishedMealVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            finishedMealVisual.transform.SetParent(plateTransform);
            finishedMealVisual.transform.localPosition = Vector3.zero;
            finishedMealVisual.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            finishedMealVisual.GetComponent<Renderer>().material.color = Color.magenta;
        }
    }
}
