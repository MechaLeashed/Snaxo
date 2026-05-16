using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float interactDistance = 2.0f;
    public Transform raycastOrigin; // Drag the "RaycastOrigin" object here!
    public Transform holdPoint;

    private GameObject currentItemObject;
    private IngredientData currentItemData;

    void Update()
    {
        // New Input System check for the "E" key
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

  
    private void TryInteract()
    {
        // Now we use the exact position and direction of our helper object
        Vector3 origin = raycastOrigin.position;
        Vector3 direction = raycastOrigin.forward;

        Debug.DrawRay(origin, direction * interactDistance, Color.red, 1.0f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, interactDistance))
        {
            Debug.Log("Hit: " + hit.collider.name);

            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(this);
            }
        }
    }

    // Helper functions for the kitchen stations to call
    public bool IsHoldingItem() => currentItemObject != null;

    public void PickUpItem(GameObject obj, IngredientData data)
    {
        currentItemObject = obj;
        currentItemData = data;

        obj.transform.SetParent(holdPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        
        if (obj.TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;
    }

    public void ClearHand()
    {
        currentItemObject = null;
        currentItemData = null;
    }

    public IngredientData GetHeldItem() => currentItemData;
    public GameObject GetCurrentObject() => currentItemObject;
}