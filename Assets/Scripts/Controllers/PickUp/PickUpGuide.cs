using FishNet.Object;
using UnityEngine;

namespace Controllers.PickUp
{
    public class PickUpGuide : NetworkBehaviour
    {
        [Header("References")] 
        [SerializeField] private Transform pickUpTransform; // The position where picked objects will be held

        [Header("Config")] 
        [SerializeField] private float rayDistance = 2f; // Raycast range for detecting objects

        [Header("Debug")] 
        private Camera _camera; // Reference to the main camera
        [SerializeField] private bool _hasObjectInHand; // Tracks if the player is holding an object
        [SerializeField] private GameObject _objInHand; // Reference to the currently held object

        // Input keys
        private KeyCode pickUpKey = KeyCode.E;
        private KeyCode dropKey = KeyCode.Q;

        public override void OnStartClient()
        {
            base.OnStartClient();

            // Disable script for non-owners to ensure only the local player controls this
            if (!IsOwner) enabled = false;

            _camera = Camera.main;
        }

        private void Update()
        {
            // Listen for input
            if (Input.GetKeyDown(pickUpKey)) PickUp();
            if (Input.GetKeyDown(dropKey)) Drop();
        }

        #region Pick Up

        private void PickUp()
        {
            // Shoot a ray from the camera to detect pickable objects
            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hit, rayDistance))
            {
                if (!hit.collider.transform.TryGetComponent(out PickableObject pickableObject)) return;

                // If hand is empty, pick up the object
                if (!_hasObjectInHand)
                {
                    Debug.LogWarning("Picking up object");
                    PickUpObjectServer(pickableObject.gameObject, pickUpTransform.position, pickUpTransform.rotation, gameObject);

                    _objInHand = pickableObject.gameObject;
                    _hasObjectInHand = true;
                }
                else // If already holding something, drop current and pick new
                {
                    Drop();
                    PickUpObjectServer(pickableObject.gameObject, pickUpTransform.position, pickUpTransform.rotation, gameObject);

                    _objInHand = pickableObject.gameObject;
                    _hasObjectInHand = true;
                }
            }
        }

        // Called on the server to handle pick-up logic
        [ServerRpc(RequireOwnership = false)]
        private void PickUpObjectServer(GameObject obj, Vector3 position, Quaternion rotation, GameObject player)
        {
            // Informs all clients to visually update the pick-up
            PickSetObjectObserver(obj, position, rotation, player);
        }

        // Updates object state on all clients (position, rotation, parent, and physics)
        [ObserversRpc]
        private void PickSetObjectObserver(GameObject obj, Vector3 position, Quaternion rotation, GameObject player)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.parent = player.transform;

            // Make sure object doesn't fall due to physics while held
            obj.GetComponent<Rigidbody>().isKinematic = true;
        }

        #endregion

        #region Drop

        private void Drop()
        {
            if (!_hasObjectInHand) return;

            // Request server to drop the object
            DropObjectInHandServer(_objInHand);
            _hasObjectInHand = false;
            _objInHand = null;
        }

        // Informs the server to handle drop logic
        [ServerRpc(RequireOwnership = false)]
        private void DropObjectInHandServer(GameObject obj)
        {
            DropObjectInHandObserver(obj); // Notifies all observers
        }

        // Resets the object's parent and enables physics so it can fall/move
        [ObserversRpc]
        private void DropObjectInHandObserver(GameObject obj)
        {
            if (obj == null) return;
            obj.transform.parent = null;
            obj.GetComponent<Rigidbody>().isKinematic = false;
        }

        #endregion
    }
}
