using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    public Transform cam; // Drag your main camera here

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    // --- REMAPPABLE KEYCODE VARIABLES (THESE ARE WHAT FIX THE CS1061 ERRORS!) ---
    // These must be present in PlayerMovement.cs for GameUIController to access them.
    public KeyCode moveForwardKey = KeyCode.W;
    public KeyCode moveBackwardKey = KeyCode.S;
    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    // --- END REMAPPABLE KEYCODE VARIABLES ---

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cam == null)
        {
            cam = Camera.main.transform;
            if (cam == null)
            {
                Debug.LogError("PlayerMovement: Main Camera not found! Please tag your camera as 'MainCamera' or assign it manually.");
            }
        }
        // Key bindings will be loaded and applied by GameUIController's LoadKeyBindings()
        // No need to load them directly here, as GameUIController will handle it.
    }

    void Update()
    {
        // --- Ground Check ---
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Stick to the ground
        }

        // --- Input from Remappable Keys ---
        float h = 0f; // Horizontal input value (A/D or Left/Right remapped keys)
        float v = 0f; // Vertical input value (W/S or Up/Down remapped keys)

        if (Input.GetKey(moveRightKey)) h = 1f;
        if (Input.GetKey(moveLeftKey)) h = -1f;

        if (Input.GetKey(moveForwardKey)) v = 1f;
        if (Input.GetKey(moveBackwardKey)) v = -1f;

        // Normalize inputDir to prevent faster diagonal movement (e.g., when pressing W+A)
        Vector3 inputDir = new Vector3(h, 0f, v).normalized;

        // --- Sprint ---
        float currentSpeed = Input.GetKey(sprintKey) ? sprintSpeed : walkSpeed;

        // --- Movement relative to camera ---
        if (inputDir.magnitude >= 0.1f) // Only move if there's significant input (to prevent tiny movements from floating point errors)
        {
            // Calculate target angle based on camera's forward direction
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            Vector3 moveDir = rotation * Vector3.forward; // Calculate actual movement direction based on angle

            // Apply movement using CharacterController
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

            // Rotate player toward movement direction (optional, but good for player models)
            // Slerp for smooth rotation transition
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10f);
        }

        // --- Jump ---
        // Use GetKeyDown for single jump on key press (prevents holding jump to fly)
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            // Physics formula to calculate initial vertical velocity needed to reach jumpHeight
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // --- Gravity ---
        velocity.y += gravity * Time.deltaTime; // Apply gravity over time
        controller.Move(velocity * Time.deltaTime); // Apply vertical velocity (gravity & jump)
    }
}