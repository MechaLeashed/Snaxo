using UnityEngine;

public class PantryManager : MonoBehaviour, IInteractable
{
    [Header("UI References")]
    public GameObject pantryUI;

    // We manually assign the 3 buttons already in the scene
    public PantryButton slot1;
    public PantryButton slot2;
    public PantryButton slot3;

    [Header("Ingredient Data")]
    public IngredientData item1;
    public IngredientData item2;
    public IngredientData item3;

    private PlayerController interactingPlayer;

    void Start()
    {
        pantryUI.SetActive(false);

        // Wire up the buttons manually at the start
        if (slot1 != null && item1 != null) slot1.Setup(item1, this);
        if (slot2 != null && item2 != null) slot2.Setup(item2, this);
        if (slot3 != null && item3 != null) slot3.Setup(item3, this);
    }

    public void Interact(PlayerController player)
    {
        interactingPlayer = player;
        pantryUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GiveItemToPlayer(IngredientData data)
    {
        if (interactingPlayer != null && !interactingPlayer.IsHoldingItem())
        {
            GameObject newFood = Instantiate(data.prefab);
            interactingPlayer.PickUpItem(newFood, data);
            CloseMenu();
        }
    }

    public void CloseMenu()
    {
        pantryUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}