using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CuttingBoard : MonoBehaviour, IInteractable
{
    [Header("Global UI")]
    public GameObject entireUiBackground;
    public Slider progressSlider; 
    public Image activeFoodIcon;  

    [Header("Queue Setup")]
    [Tooltip("Assign 3 Transforms here in order (Slot 0 = Front/Chopping, Slot 1 = Middle, Slot 2 = Back)")]
    public List<Transform> queueAnchors = new List<Transform>();

    [Header("Settings")]
    [Range(0.1f, 5f)] public float globalChopSpeedMultiplier = 1.0f;

    // Internal data structures to track our queue
    private List<IngredientData> itemQueue = new List<IngredientData>();
    private List<GameObject> visualQueue = new List<GameObject>();
    
    private float chopTimer = 0f;
    private bool isPlayerZoneActive = false; // Tracks if a player is standing in front of the board

    private void Start()
    {
        if (entireUiBackground != null) entireUiBackground.SetActive(false);
    }

    public void Interact(PlayerController player)
    {
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
        if (itemQueue.Count >= 3) return;

        IngredientData item = player.GetHeldItem();
        if (item == null || item.processedResult == null) return;

        itemQueue.Add(item);
        
        GameObject foodVisual = player.GetCurrentObject();
        visualQueue.Add(foodVisual);

        int currentSlotIndex = itemQueue.Count - 1;
        UpdateObjectPlacement(foodVisual, currentSlotIndex);

        player.ClearHand();

        if (itemQueue.Count == 1)
        {
            ResetActiveProgressUI();
        }

        UpdateUiVisibility();
    }

    private void TryPickUpFood(PlayerController player)
    {
        if (itemQueue.Count > 0 && chopTimer >= itemQueue[0].cookTime)
        {
            IngredientData finishedData = itemQueue[0].processedResult;
            
            Destroy(visualQueue[0]);

            GameObject finishedFood = Instantiate(finishedData.prefab);
            player.PickUpItem(finishedFood, finishedData);

            itemQueue.RemoveAt(0);
            visualQueue.RemoveAt(0);

            ShiftQueueForward();

            chopTimer = 0f;
            if (itemQueue.Count > 0)
            {
                ResetActiveProgressUI();
            }

            UpdateUiVisibility();
        }
    }

    private void Update()
    {
        // Only allow chopping if there are items, it's not done, AND the player is standing here
        if (itemQueue.Count > 0 && chopTimer < itemQueue[0].cookTime && isPlayerZoneActive)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.isPressed)
            {
                ProcessChopping();
            }
        }
    }

    private void ProcessChopping()
    {
        IngredientData activeItem = itemQueue[0];
        chopTimer += Time.deltaTime * globalChopSpeedMultiplier;

        if (progressSlider != null)
        {
            progressSlider.value = chopTimer;
        }

        if (chopTimer >= activeItem.cookTime)
        {
            chopTimer = activeItem.cookTime;
            
            Destroy(visualQueue[0]);
            GameObject choppedVisual = Instantiate(activeItem.processedResult.prefab);
            visualQueue[0] = choppedVisual;
            UpdateObjectPlacement(choppedVisual, 0);

            if (progressSlider != null) progressSlider.gameObject.SetActive(false);
        }
    }

    // --- PROXIMITY DETECTION ---
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the zone is the Player
        if (other.GetComponent<PlayerController>() != null)
        {
            isPlayerZoneActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object leaving the zone is the Player
        if (other.GetComponent<PlayerController>() != null)
        {
            isPlayerZoneActive = false;
        }
    }
    // ----------------------------

    private void ShiftQueueForward()
    {
        for (int i = 0; i < visualQueue.Count; i++)
        {
            UpdateObjectPlacement(visualQueue[i], i);
        }
    }

    private void UpdateObjectPlacement(GameObject obj, int slotIndex)
    {
        if (slotIndex < queueAnchors.Count && queueAnchors[slotIndex] != null)
        {
            obj.transform.SetParent(queueAnchors[slotIndex]);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
        }
    }

    private void ResetActiveProgressUI()
    {
        chopTimer = 0f;
        if (itemQueue.Count > 0)
        {
            if (activeFoodIcon != null) activeFoodIcon.sprite = itemQueue[0].icon;
            if (progressSlider != null)
            {
                progressSlider.gameObject.SetActive(true);
                progressSlider.minValue = 0;
                progressSlider.maxValue = itemQueue[0].cookTime;
                progressSlider.value = 0;
            }
        }
    }

    private void UpdateUiVisibility()
    {
        if (entireUiBackground != null)
        {
            entireUiBackground.SetActive(itemQueue.Count > 0);
        }
    }
}