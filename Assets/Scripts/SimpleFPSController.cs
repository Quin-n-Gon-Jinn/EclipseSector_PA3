using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleFPSController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4.5f;
    public float sprintSpeed = 7f;
    public float gravity = -20f;
    public float jumpHeight = 1.1f;

    [Header("Look")]
    public Camera playerCamera;
    public float mouseSensitivity = 0.12f;
    public float maxLookAngle = 80f;

    private CharacterController controller;
    private float verticalVelocity;
    private float pitch;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame &&
            Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void HandleMovement()
    {
        if (Keyboard.current == null)
            return;

        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) input.y += 1f;
        if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        if (Keyboard.current.dKey.isPressed) input.x += 1f;
        if (Keyboard.current.aKey.isPressed) input.x -= 1f;

        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 move = (transform.right * input.x + transform.forward * input.y);

        float speed = Keyboard.current.leftShiftKey.isPressed ? sprintSpeed : moveSpeed;

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 velocity = move * speed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        if (Mouse.current == null || playerCamera == null || Cursor.lockState != CursorLockMode.Locked)
            return;

        Vector2 delta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        transform.Rotate(Vector3.up * delta.x);

        pitch -= delta.y;
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
