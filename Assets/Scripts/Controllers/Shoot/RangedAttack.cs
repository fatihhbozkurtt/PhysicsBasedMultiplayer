using FishNet.Object;
using UnityEngine;

namespace Controllers.Shoot
{
    /// <summary>
    /// Handles ranged weapon attacks in a networked game using FishNet.
    /// Responsible for input detection, firing cooldown, and spawning projectiles on the server.
    /// </summary>
    public class RangedAttack : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private NetworkObject projectilePrefab; // The networked projectile to spawn

        [Header("Config")]
        [SerializeField] private float projectileSpeed = 3f;     // Speed at which the projectile travels
        [SerializeField] private float fireCooldown = 0.5f;      // Time between allowed shots

        private float _lastFireTime; // Last time the player shot

        /// <summary>
        /// Checks for player input to initiate a ranged attack.
        /// Only the owner of the object can perform the shooting.
        /// </summary>
        private void Update()
        {
            if (!IsOwner) return;

            if (Input.GetKeyDown(KeyCode.Mouse0))
                TryShoot();
        }

        /// <summary>
        /// Validates cooldown before requesting a server-side shoot.
        /// </summary>
        private void TryShoot()
        {
            // Prevents firing before cooldown is complete
            if (Time.time - _lastFireTime < fireCooldown)
                return;

            _lastFireTime = Time.time;

            // Calculate projectile start position and direction
            Vector3 startPos = transform.position + transform.forward * 1.5f;
            Vector3 direction = transform.forward;

            // Send request to the server to spawn the projectile
            RequestShootServer(startPos, direction);
        }

        /// <summary>
        /// Called on the server to spawn the projectile and initialize its movement.
        /// </summary>
        /// <param name="startPosition">The position to spawn the projectile</param>
        /// <param name="direction">The direction the projectile will travel</param>
        [ServerRpc]
        private void RequestShootServer(Vector3 startPosition, Vector3 direction)
        {
            // Instantiate the networked projectile on the server
            NetworkObject projectileInstance = Instantiate(projectilePrefab, startPosition, Quaternion.identity);
            
            // Initialize the projectile with movement parameters
            projectileInstance.GetComponent<Projectile>().Init(direction, projectileSpeed);

            // Spawn the projectile across the network so all clients can see it
            Spawn(projectileInstance);
        }
    }
}
