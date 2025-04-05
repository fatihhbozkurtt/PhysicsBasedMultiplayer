using FishNet.Object;
using UnityEngine;

namespace Controllers
{
    public class PlayerController : NetworkBehaviour
    {
        [Header("Configuration")] public float walkSpeed = 5f;
        public float runSpeed = 10f;
        public float jumpHeight = 2f;
        public float gravity = -9.81f;
        public Transform cameraTransform;
        public float mouseSensitivity = 100f;

        [Header("Debug")] public Camera playerCamera;
        private CharacterController controller;
        private Vector3 velocity;
        private bool isGrounded;

        private float xRotation = 0f;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsOwner)
            {
                // Do not prefer to spawn multiple cameras
                // Instead get the main camera and set it as player's local camera
                // Thus it prevents us to isntantiate cameras
                playerCamera = Camera.main;
                playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + 0.4f,
                    transform.position.z + .25f);
                playerCamera.transform.SetParent(transform);
            }
            else
            {
                // disable if we are not the owner
                // so we cant control other players' scripts
                GetComponent<PlayerController>().enabled = false;
            }

            cameraTransform = Camera.main.transform;
        }

        void Start()
        {
            controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked; // Fare ekran ortasında gizli
        }

        void Update()
        {
            // Zemin kontrolü
            isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            // WASD ile hareket
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 move = transform.right * x + transform.forward * z;

            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            controller.Move(move * (currentSpeed * Time.deltaTime));

            // Zıplama
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Yerçekimi
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            // Q / E tuşları ile karakterin yönünü çevirme
            float rotationSpeed = 100f; // Derece/saniye
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }
        }
    }
}