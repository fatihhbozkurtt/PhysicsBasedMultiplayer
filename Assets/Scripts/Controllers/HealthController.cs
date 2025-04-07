using Data;
using FishNet.Object;
using TMPro;
using UnityEngine;

namespace Controllers
{
    public class HealthController : NetworkBehaviour, IDamageable
    {
        [Header("References")] 
        [SerializeField] private TextMeshProUGUI hpText;

        [Header("Configuration")] 
        [SerializeField] private int maxHealth;

        [Header("Debug")] 
        [SerializeField] private int currentHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
            UpdateHealthUI();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            // Only allow owner to interact with health
            if (!IsOwner)
                enabled = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                RequestHealthChangeServer(-1);
        }
        
        public void TakeDamage(int amount)
        {
            // The method is being run on the server. 'this' refers to the correct HealthController
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }
        
        [ServerRpc] 
        public void RequestHealthChangeServer(int amountToChange)
        {
            TakeDamage(amountToChange);
            // Update all clients with the new value for this object
            UpdateHealthObserver(currentHealth);
        }


        [ObserversRpc]
        private void UpdateHealthObserver(int newHealth)
        {
            currentHealth = newHealth;
            UpdateHealthUI();
        }

        private void UpdateHealthUI()
        {
            if (hpText != null)
                hpText.text = currentHealth.ToString();
        }

      
    }
}