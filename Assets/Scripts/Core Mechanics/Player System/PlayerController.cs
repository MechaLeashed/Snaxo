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
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. Update the raycast direction based on movement
        UpdateRaycastDirection();

        // 2. New Input System check for the "E" key
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

    private void UpdateRaycastDirection()
    {
        if (Keyboard.current == null) return;

        // 1. Read the state of all 4 direction keys
        bool moveNorth = Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed;
        bool moveSouth = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed;
        bool moveEast  = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed;
        bool moveWest  = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed;

        // 2. Map directions to your exact requested rotation angles
        if (moveNorth)
        {
            // Face North -> Raycast rotation stays at 0
            raycastOrigin.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (moveSouth)
        {
            // Face South -> Raycast rotation switches to 180
            raycastOrigin.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else if (moveEast)
        {
            // Face East -> Raycast rotation switches to 90
            raycastOrigin.localRotation = Quaternion.Euler(0, 90, 0);
        }
        else if (moveWest)
        {
            // Face West -> Raycast rotation switches to -90 (or 270)
            raycastOrigin.localRotation = Quaternion.Euler(0, -90, 0);
        }
    }

    private void TryInteract()
    {
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