using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class ChopSlot
{
    [Header("Physical Setup")]
    public Transform anchor;
    public GameObject uiPanel;
    public Slider progressSlider;
    public Image foodIcon;

    [HideInInspector] public IngredientData currentItem;
    [HideInInspector] public GameObject visualObject;
    [HideInInspector] public float timer;
    [HideInInspector] public bool isBusy => currentItem != null;
    [HideInInspector] public bool isDone;
    [HideInInspector] public PlayerController lastWorker; // Tracks who placed the food
}

public class CuttingBoard : MonoBehaviour, IInteractable
{
    [Header("Chopping Configuration")]
    public List<ChopSlot> slots = new List<ChopSlot>();

    [Header("Settings")]
    [Range(0.1f, 5f)] public float globalChopSpeedMultiplier = 1.0f;

    private void Start()
    {
        // Hide all UI panels on start
        foreach (var slot in slots)
        {
            if (slot.uiPanel != null) slot.uiPanel.SetActive(false);
        }
    }

    public void Interact(PlayerController player)
    {
        // Logic: If holding food -> Place it. If hands empty -> Try pick up manually.
        if (player.IsHoldingItem())
        {
            TryPlaceFood(player);
        }
        else
        {
            TryPickUpFood(player);
        }
    }

    private void TryPlaceFood(PlayerController player)
    {
        IngredientData item = player.GetHeldItem();

        // Safety: Only proceed if item is choppable
        if (item == null || item.processedResult == null) return;

        foreach (var slot in slots)
        {
            if (!slot.isBusy)
            {
                slot.lastWorker = player; // Link player to this slot
                slot.currentItem = item;
                slot.visualObject = player.GetCurrentObject();

                slot.visualObject.transform.SetParent(slot.anchor);
                slot.visualObject.transform.localPosition = Vector3.zero;
                slot.visualObject.transform.localRotation = Quaternion.identity;

                player.ClearHand();

                slot.timer = 0;
                slot.isDone = false;
                slot.uiPanel.SetActive(true);

                if (slot.foodIcon != null) slot.foodIcon.sprite = item.icon;
                if (slot.progressSlider != null)
                {
                    slot.progressSlider.minValue = 0;
                    slot.progressSlider.maxValue = item.cookTime;
                    slot.progressSlider.value = 0;
                }
                return;
            }
        }
    }

    private void TryPickUpFood(PlayerController player)
    {
        // Manual pickup if the auto-pickup failed (e.g. player's hands were full when it finished)
        foreach (var slot in slots)
        {
            if (slot.isBusy && slot.isDone)
            {
                player.PickUpItem(slot.visualObject, slot.currentItem);

                slot.uiPanel.SetActive(false);
                slot.currentItem = null;
                slot.visualObject = null;
                slot.isDone = false;
                slot.lastWorker = null;
                return;
            }
        }
    }

    private void Update()
    {
        foreach (var slot in slots)
        {
            if (slot.isBusy && !slot.isDone)
            {
                slot.timer += Time.deltaTime * globalChopSpeedMultiplier;

                if (slot.progressSlider != null)
                {
                    slot.progressSlider.value = slot.timer;
                }

                if (slot.timer >= slot.currentItem.cookTime)
                {
                    FinishSlot(slot);
                }
            }
        }
    }

    private void FinishSlot(ChopSlot slot)
    {
        slot.isDone = true;
        IngredientData choppedData = slot.currentItem.processedResult;

        // 1. Remove the raw model
        Destroy(slot.visualObject);

        // 2. Create the chopped model
        GameObject finishedFood = Instantiate(choppedData.prefab);

        // 3. Try Auto-Pickup
        if (slot.lastWorker != null && !slot.lastWorker.IsHoldingItem())
        {
            slot.lastWorker.PickUpItem(finishedFood, choppedData);

            // Clean up slot immediately
            slot.uiPanel.SetActive(false);
            slot.currentItem = null;
            slot.visualObject = null;
            slot.lastWorker = null;
            slot.isDone = false;
        }
        else
        {
            // If player hands are full, leave it on the board for manual pickup
            finishedFood.transform.SetParent(slot.anchor);
            finishedFood.transform.localPosition = Vector3.zero;
            finishedFood.transform.localRotation = Quaternion.identity;

            slot.visualObject = finishedFood;
            slot.currentItem = choppedData;
            // Keep uiPanel active but maybe change color to show it's done
        }
    }
}