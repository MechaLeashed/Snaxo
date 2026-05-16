using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Kitchen/Recipe")]
public class RecipeData : ScriptableObject
{
    [Header("Recipe Details")]
    public string recipeName;
    
    [Header("Requirements")]
    [Tooltip("The ingredients needed to make this meal.")]
    public List<IngredientData> requiredIngredients;
    
    [Header("Result")]
    [Tooltip("The finished meal ingredient data (which contains the prefab and icon of the final food).")]
    public IngredientData finishedMeal;
}
