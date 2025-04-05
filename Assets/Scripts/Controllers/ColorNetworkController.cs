using FishNet.Object;
using UnityEngine;

namespace Controllers
{
    public class ColorNetworkController : NetworkBehaviour
    {
        [Header("References")] public Renderer meshRenderer;

        [Header("Configuration")] [SerializeField]
        private Color endColor;

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
            if (Input.GetKeyDown(KeyCode.F))
                ChangeColorServer(this, endColor);
        }


        [ServerRpc]
        public void ChangeColorServer(ColorNetworkController cnc, Color color)
        {
            RpcSetColor(cnc, color);
        }

        [ObserversRpc]
        public void RpcSetColor(ColorNetworkController cnc, Color color)
        {
            cnc.meshRenderer.material.color = color;
        }
    }
}