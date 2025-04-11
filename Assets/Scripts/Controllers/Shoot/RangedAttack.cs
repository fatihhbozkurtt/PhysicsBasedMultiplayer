using FishNet.Object;
using UnityEngine;

namespace Controllers.Shoot
{
    public class RangedAttack : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private NetworkObject projectilePrefab;

        [Header("Config")]
        [SerializeField] private float projectileSpeed = 3f;
        [SerializeField] private float fireCooldown = 0.5f;

        private float _lastFireTime;

        private void Update()
        {
            if (!IsOwner) return;

            if (Input.GetKeyDown(KeyCode.Mouse0))
                TryShoot();
        }

        private void TryShoot()
        {
            if (Time.time - _lastFireTime < fireCooldown)
                return;

            _lastFireTime = Time.time;

            Vector3 startPos = transform.position + transform.forward * 1.5f;
            Vector3 direction = transform.forward;

            RequestShootServer(startPos, direction);
        }

        [ServerRpc]
        private void RequestShootServer(Vector3 startPosition, Vector3 direction)
        {
            NetworkObject projectileInstance = Instantiate(projectilePrefab, startPosition, Quaternion.identity);
            
            // yön bilgisi vs. projectile’a gönderiliyor
            projectileInstance.GetComponent<Projectile>().Init(direction, projectileSpeed);

            Spawn(projectileInstance);
        }
    }
}