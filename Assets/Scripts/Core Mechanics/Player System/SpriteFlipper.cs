using UnityEngine;

public class SpriteFlipper : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private CharacterController characterController;
    private Vector3 lastPosition;

    private void Start()
    {
        // 1. Grab the SpriteRenderer on this object
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 2. Try to find the CharacterController on this object or its parent
        characterController = GetComponentInParent<CharacterController>();
        
        // Initialize our position tracker
        lastPosition = transform.position;
    }

    private void Update()
    {
        float horizontalVelocity = 0f;

        // Method A: If a CharacterController is attached, use its actual velocity vector
        if (characterController != null)
        {
            horizontalVelocity = characterController.velocity.x;
        }
        // Method B: Fallback calculation based on frame-by-frame world position changes
        else
        {
            horizontalVelocity = transform.position.x - lastPosition.x;
            lastPosition = transform.position;
        }

        // 3. Flip the sprite based on the direction of horizontal movement
        // (A small threshold prevents unintentional flipping from micro-movements)
        if (horizontalVelocity > 0.01f)
        {
            spriteRenderer.flipX = false; // Moving Right -> Face Right
        }
        else if (horizontalVelocity < -0.01f)
        {
            spriteRenderer.flipX = true;  // Moving Left -> Face Left
        }
    }
}