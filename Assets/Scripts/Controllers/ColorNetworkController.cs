using FishNet.Object;
using UnityEngine;

namespace Controllers
{
    /// <summary>
    /// Controls color change on networked player objects using FishNet.
    /// Handles ownership checks and synchronizes material color across all clients.
    /// </summary>
    public class ColorNetworkController : NetworkBehaviour
    {
        [Header("References")] 
        public Renderer meshRenderer; // Renderer whose material color will be changed

        [Header("Configuration")]
        [SerializeField] private Color endColor; // The target color to change to

        /// <summary>
        /// Called when the client starts. Disables control scripts if the player is not the owner.
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner)
            {
                // Disable the PlayerController for non-owners to prevent controlling other players
                GetComponent<PlayerController>().enabled = false;
            }
        }

        /// <summary>
        /// Checks for input from the local player. Initiates a color change when 'F' is pressed.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
                ChangeColorServer(this, endColor); // Requests the server to handle the color change
        }

        /// <summary>
        /// Server-side function to handle color change requests.
        /// Calls an RPC to update the color on all clients.
        /// </summary>
        /// <param name="cnc">The ColorNetworkController of the object to change</param>
        /// <param name="color">The new color to set</param>
        [ServerRpc]
        public void ChangeColorServer(ColorNetworkController cnc, Color color)
        {
            RpcSetColor(cnc, color);
        }

        /// <summary>
        /// Called on all clients to apply the color change.
        /// </summary>
        /// <param name="cnc">The ColorNetworkController of the object to change</param>
        /// <param name="color">The color to apply to the mesh renderer</param>
        [ObserversRpc]
        public void RpcSetColor(ColorNetworkController cnc, Color color)
        {
            cnc.meshRenderer.material.color = color;
        }
    }
}
