using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace Controllers
{
    /// <summary>
    /// This controller handles the spawning and despawning of objects in a multiplayer game.
    /// The local player can spawn and despawn balls, and the server ensures synchronization across clients.
    /// </summary>
    public class SpawnNetworkController : NetworkBehaviour
    { 
        [Header("References")]
        public GameObject ObjectToSpawnPrefab; // Prefab to spawn (e.g., ball object)

        [Header("Debug")]
        public List<GameObject> spawnedBalls; // List of spawned objects (balls)

        /// <summary>
        /// Called when the client starts. Only the owner can control the spawning logic.
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner)
            {
                // Disable the PlayerController for non-owners so they cannot control the object
                GetComponent<PlayerController>().enabled = false;
            }
        }

        /// <summary>
        /// Update method to check for input to spawn or despawn objects.
        /// </summary>
        private void Update()
        {
            // Check if the owner presses the number keys to spawn or despawn a ball
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SpawnBallServer(ObjectToSpawnPrefab, transform.position + (transform.forward * 2), this); // Spawn
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                // If there are spawned balls, despawn the last one
                if (spawnedBalls.Count <= 0) return;

                GameObject lastBall = spawnedBalls[^1];
                spawnedBalls.Remove(lastBall);
                DespawnBallServer(lastBall); // Despawn
            }
        }

        /// <summary>
        /// Adds a newly spawned ball to the list of spawned balls (on the client side).
        /// </summary>
        private void AddSpawnedBall(GameObject newBall)
        {
            if (spawnedBalls.Contains(newBall)) return;
            spawnedBalls.Add(newBall); // Add to the list if not already present
        }

        #region Spawn

        /// <summary>
        /// ServerRpc method to spawn a ball object on the server.
        /// The ball is instantiated, and a random color is applied.
        /// </summary>
        [ServerRpc]
        private void SpawnBallServer(GameObject prefab, Vector3 spawnPoint, SpawnNetworkController invoker)
        {
            GameObject clone = Instantiate(prefab, spawnPoint, Quaternion.identity);
            ServerManager.Spawn(clone); // Spawn on the server

            // Set a random color for the ball
            Color randomColor = GetRandomColor();

            // Send the ball object and its color to clients
            SetSpawnedBallClient(clone, invoker, randomColor);
        }

        /// <summary>
        /// ObserversRpc method to notify all clients about the newly spawned ball.
        /// The spawned object is added to the client's list and its color is applied.
        /// </summary>
        [ObserversRpc]
        private void SetSpawnedBallClient(GameObject spawnedObj, SpawnNetworkController invoker, Color color)
        {
            invoker.AddSpawnedBall(spawnedObj); // Add the spawned ball to the local list

            // Apply the random color to the ball's material
            MeshRenderer renderer = spawnedObj.GetComponent<MeshRenderer>();
            if (renderer != null)
                renderer.material.color = color; // Set color on the mesh
        }

        /// <summary>
        /// Randomly generates a color to be applied to the spawned object.
        /// </summary>
        private Color GetRandomColor()
        {
            return Random.ColorHSV(); // Generates a random color in HSV space
        }
        #endregion

        #region Despawn

        /// <summary>
        /// ServerRpc method to despawn a ball object from the server.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void DespawnBallServer(GameObject spawnedBall)
        {
            ServerManager.Despawn(spawnedBall); // Despawn the object on the server
        }
        #endregion
    }
}
