using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIInventory : MonoBehaviour
{
    public TMP_Text inventoryText;

    public void UpdateUI(Dictionary<string, int> items)
    {
        inventoryText.text = "";

        foreach (var item in items)
        {
            inventoryText.text += item.Key + ": " + item.Value + "\n";
        }
    }
}