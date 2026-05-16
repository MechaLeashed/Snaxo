using UnityEngine;

public class SpriteDirectionController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Minimum speed to trigger a sprite change")]
    public float movementThreshold = 0.1f;

    [Header("Sprites")]
    public Sprite frontSprite; // Moving toward camera (-Z)
    public Sprite backSprite;  // Moving away from camera (+Z)
    public Sprite sideSprite;  // Moving Left/Right (X)

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // Automatically find the SpriteRenderer on the "Plane" child if not assigned
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        UpdateSpriteDirection();
    }

    private void UpdateSpriteDirection()
    {
        if (controller == null) return;

        // CharacterController.velocity gives us the real movement speed in 3D space
        Vector3 velocity = controller.velocity;
        
        // We only care about horizontal movement (X and Z)
        float moveX = velocity.x;
        float moveZ = velocity.z;

        // Check if the player is actually moving
        if (new Vector2(moveX, moveZ).magnitude > movementThreshold)
        {
            // Logic: Is the move more Horizontal (Left/Right) or Vertical (Forward/Back)?
            if (Mathf.Abs(moveX) > Mathf.Abs(moveZ))
            {
                // Horizontal Movement
                spriteRenderer.sprite = sideSprite;
                spriteRenderer.flipX = (moveX < 0); // Flip if moving Left
            }
            else
            {
                // Vertical Movement
                spriteRenderer.sprite = (moveZ > 0) ? backSprite : frontSprite;
                spriteRenderer.flipX = false; // Reset flip for forward/backward
            }
        }
    }
}