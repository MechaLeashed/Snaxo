using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemName;
    public int amount = 1;

    private void OnTriggerEnter(Collider other)
    {
        Inventory inventory = other.GetComponent<Inventory>();

        if (inventory != null)
        {
            inventory.AddItem(itemName, amount);
            Destroy(gameObject);
        }
    }
}