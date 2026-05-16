using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NPCOrder : MonoBehaviour, IInteractable
{
    [Header("Order Settings")]
    [Tooltip("List of possible meals the NPC might ask for.")]
    public IngredientData[] possibleMeals;

    private IngredientData currentWantedMeal;

    [Header("UI References")]
    [Tooltip("The canvas or panel that holds the order bubble.")]
    public GameObject uiPanel;
    [Tooltip("The image component to display the wanted meal's icon.")]
    public Image orderBubbleImage;

    private bool isSatisfied = false;

    private void Start()
    {
        GenerateNewOrder();
    }

    private void GenerateNewOrder()
    {
        if (possibleMeals != null && possibleMeals.Length > 0)
        {
            // Pick a random meal from the list
            int randomIndex = Random.Range(0, possibleMeals.Length);
            currentWantedMeal = possibleMeals[randomIndex];

            if (orderBubbleImage != null)
            {
                orderBubbleImage.sprite = currentWantedMeal.icon;
            }

            uiPanel.SetActive(true);
            isSatisfied = false;
        }
        else
        {
            Debug.LogWarning("NPC has no possible meals assigned in the Inspector!");
        }
    }

    public void Interact(PlayerController player)
    {
        if (isSatisfied || currentWantedMeal == null) return; // Ignore if already fed or no meal set

        // Check if the player is holding something
        if (player.IsHoldingItem())
        {
            IngredientData heldItem = player.GetHeldItem();

            // Check if the held item matches the wanted meal
            if (heldItem == currentWantedMeal || heldItem.ingredientName == currentWantedMeal.ingredientName)
            {
                // The correct meal was given!
                Debug.Log("Player gave the correct meal! NPC is happy.");

                // Destroy the physical item the player was holding
                Destroy(player.GetCurrentObject());
                // Clear the player's hands
                player.ClearHand();

                // Satisfy the NPC
                isSatisfied = true;
                uiPanel.SetActive(false); // Hide the bubble

                // Wait 5 seconds, then generate a new order instead of destroying the NPC!
                StartCoroutine(WaitAndRespawnOrder(5f));
            }
            else
            {
                // The wrong meal was given
                Debug.Log("Wrong meal! NPC wanted: " + currentWantedMeal.ingredientName + ", but received: " + heldItem.ingredientName);
            }
        }
    }

    // Coroutine to handle the 5 second delay
    private IEnumerator WaitAndRespawnOrder(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        GenerateNewOrder();
    }
}
