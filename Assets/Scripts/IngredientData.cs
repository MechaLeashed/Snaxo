using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Kitchen/Ingredient")]
public class IngredientData : ScriptableObject
{
    public string ingredientName;
    public Sprite icon;
    public GameObject prefab;
    public IngredientData processedResult; // e.g., Tomato -> Chopped Tomato
    public float cookTime = 5f;
}