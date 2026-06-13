using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, int> items = new Dictionary<string, int>();
    public UIInventory ui;

    public void AddItem(string itemName, int amount)
    {
        if (items.ContainsKey(itemName))
            items[itemName] += amount;
        else
            items[itemName] = amount;

        ui.UpdateUI(items);

        // --- WIN CONDITION CHECK ---
        // Check if we have at least 7 Plants AND 7 Fish in our dictionary
        int plantCount = items.ContainsKey("Plant") ? items["Plant"] : 0;
        int fishCount = items.ContainsKey("Fish") ? items["Fish"] : 0;

        if (plantCount >= 7 && fishCount >= 7)
        {
            GameTimer timerScript = GameObject.FindObjectOfType<GameTimer>();
            if (timerScript != null)
            {
                timerScript.StartSceneTransition(); // Trigger the fade transition!
            }
        }
    }
}