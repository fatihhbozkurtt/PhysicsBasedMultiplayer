using System;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

namespace Controllers.Shoot
{
    public class Projectile : NetworkBehaviour
    {
        [Header("Config")] [SerializeField] private int damage = -1; // Damage dealt to the target on impact
        [SerializeField] private float lifeTime = 3f; // Time after which the projectile is destroyed

        [Header("Debug")] private Rigidbody _rb; // Rigidbody reference for physics-based movement
        private RangedAttack _rangedAttack;
        private GameObject _owner;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>(); // Cache Rigidbody component on Awake
        }

        public void Init(RangedAttack rangedAttack, GameObject owner)
        {
            _rangedAttack = rangedAttack;
            _owner = owner;
            Destroy(gameObject, lifeTime); // Ensure the projectile is destroyed after a set time (server-side only)
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.parent == transform.parent) return;

            if (other.transform.parent.TryGetComponent(out HealthController healthController))
            {
                Debug.LogWarning("Projectile Hit: " + other.transform.parent.position);
                healthController.RequestHealthChangeServer(damage); // Apply damage if the target implements HealthController
                _rangedAttack.OnProjectileDisappear(transform);
                ServerManager.Despawn(gameObject); // Despawn the projectile to remove it from the network
            }
        }

        private void OnDestroy()
        {
            _rangedAttack.OnProjectileDisappear(transform);
        }
    }
}