using Data;
using FishNet.Object;
using TMPro;
using UnityEngine;

namespace Controllers
{
    /// <summary>
    /// Manages the health of a player in a networked multiplayer game using FishNet.
    /// Health changes are handled on the server and synchronized across clients.
    /// </summary>
    public class HealthController : NetworkBehaviour, IDamageable
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI hpText; // UI Text to display current HP

        [Header("Configuration")]
        [SerializeField] private int maxHealth; // Max health of the player

        [Header("Debug")]
        [SerializeField] private int currentHealth; // Current health value (runtime)

        /// <summary>
        /// Initialize health and update UI when the object is created.
        /// </summary>
        private void Awake()
        {
            currentHealth = maxHealth;
            UpdateHealthUI();
        }

        /// <summary>
        /// Ensures only the owner can interact with health values locally.
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();

            // Disable this component if not the local player
            if (!IsOwner)
                enabled = false;
        }

        /// <summary>
        /// Debug input to simulate damage using the R key.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                RequestHealthChangeServer(-1); // Decrease health by 1 on key press
        }

        /// <summary>
        /// Called on the server to apply damage or healing.
        /// </summary>
        /// <param name="amount">Amount to change health by. Negative = damage, positive = heal</param>
        public void TakeDamage(int amount)
        {
            if (!IsServer) return;

            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            UpdateHealthObserver(currentHealth); // Sync updated health to all clients
        }

        /// <summary>
        /// Called by the client to request a health change from the server.
        /// </summary>
        /// <param name="amountToChange">Amount to change the health</param>
        [ServerRpc]
        public void RequestHealthChangeServer(int amountToChange)
        {
            Debug.LogWarning("Requesting health change: " + amountToChange);
            TakeDamage(amountToChange); // Server applies the damage
            UpdateHealthObserver(currentHealth); // Notifies all clients
        }

        /// <summary>
        /// Called on all clients to update the health value and UI.
        /// </summary>
        /// <param name="newHealth">New health value to display</param>
        [ObserversRpc]
        private void UpdateHealthObserver(int newHealth)
        {
            currentHealth = newHealth;
            UpdateHealthUI();
        }

        /// <summary>
        /// Updates the health UI text component.
        /// </summary>
        private void UpdateHealthUI()
        {
            if (hpText != null)
                hpText.text = currentHealth.ToString();
        }
    }
}
