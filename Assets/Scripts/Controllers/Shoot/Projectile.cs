using FishNet.Object;
using UnityEngine;

namespace Controllers.Shoot
{
    public class Projectile : NetworkBehaviour
    {
        private Vector3 _direction;
        private float _speed;

        public void Init(Vector3 direction, float speed)
        {
            _direction = direction;
            _speed = speed;
        }

        private void Update()
        {
            if (!IsServer) return;

            transform.position += _direction * (_speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer) return;

            // Hasar ver vs.
            if (other.transform.parent.TryGetComponent(out HealthController healthController))
            {
                Debug.Log("Projectile hit: " + other.name);
                healthController.TakeDamage(-1);
                Despawn(); // FishNet tarafından, objeyi networkten kaldırır
            }
        }
    }
}