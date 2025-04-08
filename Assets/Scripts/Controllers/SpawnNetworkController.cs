using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace Controllers
{
    public class SpawnNetworkController : NetworkBehaviour
    { 
        
        [Header("References")] public GameObject ObjectToSpawnPrefab;

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
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SpawnBallServer(ObjectToSpawnPrefab, transform.position + (transform.forward * 2), this);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (spawnedBalls.Count <= 0) return;

                GameObject lastBall = spawnedBalls[^1];
                spawnedBalls.Remove(lastBall);
                DespawnBallServer(lastBall);
            }
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

            // Sunucuda renk belirlenir
            Color randomColor = GetRandomColor();

            // Renk ve referanslar istemcilere gönderilir
            SetSpawnedBallClient(clone, invoker, randomColor);
        }

        [ObserversRpc]
        private void SetSpawnedBallClient(GameObject spawnedObj, SpawnNetworkController invoker, Color color)
        {
            // İstemcide listeye ekleme
            invoker.AddSpawnedBall(spawnedObj);

            // Materyal rengi atama
            MeshRenderer renderer = spawnedObj.GetComponent<MeshRenderer>();
            if (renderer != null)
                renderer.material.color = color;
        }

        // Bu örnekte diziyle değil direkt random renk döndürüyoruz (gerekirse dizi versiyonunu da ekleyebilirim)
        private Color GetRandomColor()
        {
            return Random.ColorHSV(); // veya sabit Color array'den seçebilirsin
        }
        #endregion

        #region Despawn

        [ServerRpc(RequireOwnership = false)]
        private void DespawnBallServer(GameObject spawnedBall)
        {
            ServerManager.Despawn(spawnedBall);
           
        }
        #endregion
    }
}