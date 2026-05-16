using UnityEngine;
using UnityEngine.UI;
using TMPro; // Remove if not using TextMeshPro

public class PantryButton : MonoBehaviour
{
    private IngredientData ingredient;
    private PantryManager manager;

    public Image iconImage;
    public TextMeshProUGUI nameLabel;

    public void Setup(IngredientData data, PantryManager pantry)
    {
        ingredient = data;
        manager = pantry;

        if (iconImage != null) iconImage.sprite = data.icon;
        if (nameLabel != null) nameLabel.text = data.ingredientName;

        // Automatically add the listener so you don't have to do it manually in the Inspector
        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (manager != null && ingredient != null)
        {
            manager.GiveItemToPlayer(ingredient);
        }
    }
}