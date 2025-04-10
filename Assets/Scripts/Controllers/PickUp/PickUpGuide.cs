using FishNet.Object;
using UnityEngine;

namespace Controllers.PickUp
{
    public class PickUpGuide : NetworkBehaviour
    {
        [Header("References")] [SerializeField]
        private Transform pickUpTransform;

        [Header("Config")] [SerializeField] private float rayDistance = 2f;

        [Header("Debug")] Camera _camera;
        [SerializeField] bool _hasObjectInHand;
        [SerializeField] GameObject _objInHand;
        KeyCode pickUpKey = KeyCode.E;
        KeyCode dropKey = KeyCode.Q;
 

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner) enabled = false;

            _camera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetKeyDown(pickUpKey)) PickUp();
            if (Input.GetKeyDown(dropKey)) Drop();
        }

        #region Pick Up

        private void PickUp()
        {
            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hit, rayDistance))
            {
                if (!hit.collider.transform.TryGetComponent(out PickableObject pickableObject)) return;

                if (!_hasObjectInHand)
                {
                    Debug.LogWarning("Picking up object");
                    PickUpObjectServer(pickableObject.gameObject, pickUpTransform.position, pickUpTransform.rotation,
                        gameObject);

                    _objInHand = pickableObject.gameObject;
                    _hasObjectInHand = true;
                }
                else
                {
                    Drop();
                    PickUpObjectServer(pickableObject.gameObject, pickUpTransform.position, pickUpTransform.rotation,
                        gameObject);

                    _objInHand = pickableObject.gameObject;
                    _hasObjectInHand = true;
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void PickUpObjectServer(GameObject obj, Vector3 position, Quaternion rotation, GameObject player)
        {
            PickSetObjectObserver(obj, position, rotation, player);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [ObserversRpc]
        private void PickSetObjectObserver(GameObject obj, Vector3 position, Quaternion rotation, GameObject player)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.parent = player.transform;

            obj.GetComponent<Rigidbody>().isKinematic = true;
        }

        #endregion

        #region Drop

        // ReSharper disable Unity.PerformanceAnalysis
        private void Drop()
        {
            if (!_hasObjectInHand) return;

            DropObjectInHandServer(_objInHand);
            _hasObjectInHand = false;
            _objInHand = null;
        }

        [ServerRpc(RequireOwnership = false)]
        private void DropObjectInHandServer(GameObject obj)
        {
            DropObjectInHandObserver(obj);
        }

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