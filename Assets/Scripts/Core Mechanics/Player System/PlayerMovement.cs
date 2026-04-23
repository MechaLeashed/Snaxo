using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 6f;
    public float runSpeed = 10f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }
    

void Update()
{
    Vector2 moveInput = Keyboard.current != null
        ? new Vector2(
            (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0),
            (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0)
          )
        : Vector2.zero;

    // Convert to 3D movements
    Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;

    // Walking and Sprint Speed
    float speed = Keyboard.current.leftShiftKey.isPressed ? runSpeed : walkSpeed;

    // Preserve Y velocity
    float yVelocity = moveDirection.y;
    moveDirection = move * speed;
    moveDirection.y = yVelocity;

    // Jump
    if (Keyboard.current.spaceKey.wasPressedThisFrame && canMove && characterController.isGrounded)
    {
        moveDirection.y = jumpPower;
    }

    // Gravity
    if (!characterController.isGrounded)
    {
        moveDirection.y -= gravity * Time.deltaTime;
    }

    characterController.Move(moveDirection * Time.deltaTime);
}
}