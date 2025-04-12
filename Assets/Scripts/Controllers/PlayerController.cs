using FishNet.Object;
using UnityEngine;

namespace Controllers
{
    /// <summary>
    /// Controls player movement and camera in a FishNet multiplayer environment.
    /// Only the local owner can move and control the camera.
    /// </summary>
    public class PlayerController : NetworkBehaviour
    {
        [Header("Configuration")]
        public float walkSpeed = 5f;          // Normal movement speed
        public float runSpeed = 10f;          // Sprint speed
        public float jumpHeight = 2f;         // Height of the jump
        public float gravity = -9.81f;        // Gravity applied to the player
        public Transform cameraTransform;     // Camera transform reference
        public float mouseSensitivity = 100f; // Mouse sensitivity for looking around

        [Header("Debug")]
        public Camera playerCamera;           // Assigned only for local player
        private CharacterController controller;
        private Vector3 velocity;             // Current vertical velocity
        private bool isGrounded;              // Is the player grounded?

        private float xRotation = 0f;         // Vertical rotation for camera

        /// <summary>
        /// Called when the local client starts. Sets up the camera only for the owner.
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsOwner)
            {
                // Use the main camera instead of instantiating new ones
                playerCamera = Camera.main;
                playerCamera.transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y + 0.4f,
                    transform.position.z + 0.3f);

                playerCamera.transform.SetParent(transform); // Attach camera to player
            }
            else
            {
                // Disable control script for non-owner clients
                GetComponent<PlayerController>().enabled = false;
            }

            cameraTransform = Camera.main.transform;
        }

        /// <summary>
        /// Initialize the CharacterController and lock the cursor.
        /// </summary>
        void Start()
        {
            controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked; // Hide and lock cursor
        }

        /// <summary>
        /// Handles movement, jumping, gravity, and camera control for the local player.
        /// </summary>
        void Update()
        {
            if (!IsOwner) return;

            // Check if grounded
            isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Reset fall velocity when grounded
            }

            // Movement input
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 move = transform.right * x + transform.forward * z;

            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            controller.Move(move * (currentSpeed * Time.deltaTime));

            // Jump input
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            // Mouse look
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp vertical rotation

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Rotate camera up/down
            transform.Rotate(Vector3.up * mouseX); // Rotate player left/right
        }
    }
}
