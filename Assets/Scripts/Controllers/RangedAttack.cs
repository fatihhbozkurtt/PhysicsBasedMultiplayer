﻿using System;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace Controllers
{
    public class RangedAttack : NetworkBehaviour
    {
        [Header("References")] [SerializeField]
        private GameObject projectilePrefab;

        [Header("Config")] [SerializeField] private float projectileSpeed = 3f;
        [SerializeField] private float fireCooldown = 0.5f;

        [Header("Debug")] private float _lastFireTime;
        private Vector3 _firePosition;
        private List<ProjectileClass> _spawnedProjectiles = new();

        private void Update()
        {
            // Everyone runs
            foreach (var p in _spawnedProjectiles)
                p.ProjectileTransform.position += p.Direction * (projectileSpeed * Time.deltaTime);

            if (!IsOwner) return;

            // Only the owner runs

            if (Input.GetKeyDown(KeyCode.Mouse0))
                Shoot();
        }

        private void Shoot()
        {
            Vector3 startPosition = transform.position + transform.forward * 0.5f + transform.up;
            Vector3 direction = transform.forward;
            
            SpawnProjectileLocal(startPosition, direction);
            SpawnProjectileServer(startPosition, direction, TimeManager.Tick);
        }

        private void SpawnProjectileLocal(Vector3 startPosition, Vector3 direction)
        {
            GameObject projectile = Instantiate(projectilePrefab, startPosition, Quaternion.identity);
            _spawnedProjectiles.Add(new ProjectileClass()
                { ProjectileTransform = projectile.transform, Direction = direction });
        }

        [ServerRpc]
        private void SpawnProjectileServer(Vector3 startPosition, Vector3 direction, uint startTick)
        {
            SpawnProjectileObserver(startPosition, direction, startTick);
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void SpawnProjectileObserver(Vector3 startPosition, Vector3 direction, uint startTick)
        {
            float timeDiff = (float)(TimeManager.Tick - startTick) / TimeManager.TickRate;
            Vector3 spawnPosition = startPosition + direction * (projectileSpeed * timeDiff);
            
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            
            _spawnedProjectiles.Add(new ProjectileClass()
                { ProjectileTransform = projectile.transform, Direction = direction });
        }
    }
}

[Serializable]
public class ProjectileClass
{
    public Transform ProjectileTransform;
    public Vector3 Direction;
}