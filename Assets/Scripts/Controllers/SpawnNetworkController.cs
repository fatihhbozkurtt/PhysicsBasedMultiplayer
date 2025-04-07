using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace Controllers
{
    public class SpawnNetworkController : NetworkBehaviour
    {
        [Header("References")] public GameObject BallPrefab;

        [Header("Debug")] public List<GameObject> spawnedBalls;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner)
            {
                // disable if we are not the owner
                // so we cant control other players' scripts
                GetComponent<PlayerController>().enabled = false;
            }
        }

        private void Update()
        {
            /*
            if (Input.GetKeyDown(KeyCode.Mouse1))
                SpawnBallServer(BallPrefab, transform.position + (transform.forward * 2), this);
            else if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                if (spawnedBalls.Count <= 0) return;

                GameObject lastBall = spawnedBalls[^1];
                spawnedBalls.Remove(lastBall);
                DespawnBallServer(lastBall);
            }
        */
        }


        private void AddSpawnedBall(GameObject newBall)
        {
            if (spawnedBalls.Contains(newBall)) return;
            spawnedBalls.Add(newBall);
        }

        #region Spawn

        [ServerRpc]
        private void SpawnBallServer(GameObject prefab, Vector3 spawnPoint, SpawnNetworkController invoker)
        {
            GameObject clone = Instantiate(prefab, spawnPoint, Quaternion.identity);
            ServerManager.Spawn(clone);
            SetSpawnedBallsList(clone, invoker);
        }

        [ObserversRpc]
        private void SetSpawnedBallsList(GameObject spawnedObj, SpawnNetworkController invoker)
        {
            invoker.AddSpawnedBall(spawnedObj);
        }

        #endregion

        #region Despawn

        [ServerRpc(RequireOwnership = false)]
        public void DespawnBallServer(GameObject spawnedBall)
        {
            ServerManager.Despawn(spawnedBall);
        }

        #endregion
    }
}