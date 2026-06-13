using UnityEngine;

public class HoldingCounter : MonoBehaviour, IInteractable
{
    [Header("Placement Setup")]
    [Tooltip("Create an empty GameObject on top of the counter and assign it here to set exactly where the item rests.")]
    public Transform itemAnchor;

    // Internal tracking fields
    private IngredientData currentItem = null;
    private GameObject visualObject = null;

    // Fast status properties
    public bool HasItem => currentItem != null;

    public void Interact(PlayerController player)
    {
        // Case 1: Counter is empty, and player is holding something -> Place it down
        if (!HasItem && player.IsHoldingItem())
        {
            PlaceItem(player);
        }
        // Case 2: Counter has an item, and player's hands are empty -> Pick it up
        else if (HasItem && !player.IsHoldingItem())
        {
            PickUpItem(player);
        }
    }

    private void PlaceItem(PlayerController player)
    {
        // Grab item data and the actual 3D object from the player's hands
        currentItem = player.GetHeldItem();
        visualObject = player.GetCurrentObject();

        if (currentItem == null || visualObject == null) return;

        // Parent the object to our counter anchor and snap it to center
        visualObject.transform.SetParent(itemAnchor);
        visualObject.transform.localPosition = Vector3.zero;
        visualObject.transform.localRotation = Quaternion.identity;

        // Clear the player's hands
        player.ClearHand();
    }

    private void PickUpItem(PlayerController player)
    {
        // Pass the object data back to the player's inventory/hand system
        player.PickUpItem(visualObject, currentItem);

        // Clear out the counter's internal memory so it can accept a new item
        currentItem = null;
        visualObject = null;
    }
}