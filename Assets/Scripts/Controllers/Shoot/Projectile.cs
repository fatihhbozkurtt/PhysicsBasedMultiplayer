using FishNet.Object;
using UnityEngine;

namespace Controllers.Shoot
{
    /// <summary>
    /// Handles projectile behavior in a networked environment.
    /// Projectiles move in a given direction, apply damage on collision with players,
    /// and are automatically destroyed after a set lifetime.
    /// </summary>
    public class Projectile : NetworkBehaviour
    {
        [SerializeField] private float lifetime = 5f; // Time before the projectile is destroyed automatically
        private Vector3 _direction; // The direction in which the projectile will travel
        private float _speed;       // The speed at which the projectile moves

        /// <summary>
        /// Called when the projectile is first created.
        /// Schedules the projectile to be destroyed after a fixed lifetime.
        /// </summary>
        private void Awake()
        {
            Destroy(gameObject, lifetime);
        }

        /// <summary>
        /// Initializes the projectile's movement parameters.
        /// This should be called after instantiating the projectile.
        /// </summary>
        /// <param name="direction">The direction of travel</param>
        /// <param name="speed">The speed of travel</param>
        public void Init(Vector3 direction, float speed)
        {
            _direction = direction;
            _speed = speed;
        }

        /// <summary>
        /// Moves the projectile forward on the server side only.
        /// </summary>
        private void Update()
        {
            if (!IsServer) return;

            transform.position += _direction * (_speed * Time.deltaTime);
        }

        /// <summary>
        /// Handles collision detection.
        /// If the projectile hits an object with a HealthController (e.g., a player), it deals damage.
        /// </summary>
        /// <param name="other">The collider the projectile hit</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer) return;

            // Check if the hit object has a HealthController component in its parent
            if (other.transform.parent.TryGetComponent(out HealthController healthController))
            {
                Debug.Log("Projectile hit: " + other.name);
                healthController.TakeDamage(-1); // Apply damage (assumed to be -1)
                Despawn(); // Remove the projectile from the network using FishNet's method
            }
        }
    }
}
