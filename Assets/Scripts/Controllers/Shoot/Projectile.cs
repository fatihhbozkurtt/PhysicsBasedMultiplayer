using FishNet.Object;
using UnityEngine;

namespace Controllers.Shoot
{
    public class Projectile : NetworkBehaviour
    {
        [Header("Config")] [SerializeField] private int damage = 1; // Damage dealt to the target on impact
        [SerializeField] private float lifeTime = 5f; // Time after which the projectile is destroyed

        [Header("Debug")] private Rigidbody _rb; // Rigidbody reference for physics-based movement

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>(); // Cache Rigidbody component on Awake
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            Destroy(gameObject, lifeTime); // Ensure the projectile is destroyed after a set time (server-side only)
        }

        /// <summary>
        /// Launches the projectile in a given direction with a specified force. Only runs on the server.
        /// </summary>
        /// <param name="direction">The direction to shoot the projectile.</param>
        /// <param name="force">The amount of force to apply.</param>
        [Server]
        public void Launch(Vector3 direction, float force)
        {
            if (_rb != null)
            {
                _rb.linearVelocity = direction.normalized * force; // Set velocity directly for consistent movement
            }
            else
            {
                Debug.LogWarning("Projectile has no Rigidbody.");
            }
        }

        /// <summary>
        /// Handles collision logic on the server. Applies damage and despawns the projectile.
        /// </summary>
        /// <param name="collision">Collision data.</param>
        private void OnCollisionEnter(Collision collision)
        {
            if (!IsServer) return; // Only the server should handle collision logic
            if (collision.transform == transform) return;

            if (collision.transform.TryGetComponent(out HealthController healthController))
            {
                Debug.Log("Projectile Hit");
                healthController.RequestHealthChangeServer(damage); // Apply damage if the target implements HealthController
                ServerManager.Despawn(gameObject); // Despawn the projectile to remove it from the network
            }
        }
    }
}