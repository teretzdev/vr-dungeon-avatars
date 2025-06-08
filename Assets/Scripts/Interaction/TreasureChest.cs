using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace DungeonYou.Interaction
{
    /// <summary>
    /// Handles treasure chest interactables that can be opened to grant loot.
    /// Integrates with the EmeraldItemSystem for item distribution.
    /// </summary>
    public class TreasureChest : MonoBehaviour
    {
        [System.Serializable]
        public class LootItem
        {
            public GameObject itemPrefab;
            [Range(0f, 1f)] public float dropChance = 0.5f;
            public int minQuantity = 1;
            public int maxQuantity = 1;
        }
        
        [Header("Chest Settings")]
        [SerializeField] private bool isLocked = false;
        [SerializeField] private string requiredKeyID = "";
        [SerializeField] private bool destroyOnEmpty = false;
        [SerializeField] private float lootSpawnRadius = 1f;
        [SerializeField] private float lootSpawnHeight = 0.5f;
        
        [Header("Loot Table")]
        [SerializeField] private List<LootItem> lootTable = new List<LootItem>();
        [SerializeField] private int goldMin = 10;
        [SerializeField] private int goldMax = 50;
        
        [Header("Animation")]
        [SerializeField] private Animator animator;
        [SerializeField] private string openAnimationTrigger = "Open";
        [SerializeField] private GameObject lidObject;
        [SerializeField] private float lidOpenAngle = -120f;
        [SerializeField] private float lidOpenSpeed = 2f;
        
        [Header("Effects")]
        [SerializeField] private ParticleSystem openParticles;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip lockedSound;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onOpen = new UnityEvent();
        [SerializeField] private UnityEvent<int> onGoldLooted = new UnityEvent<int>();
        [SerializeField] private UnityEvent<GameObject> onItemLooted = new UnityEvent<GameObject>();
        [SerializeField] private UnityEvent onLocked = new UnityEvent();
        
        private bool isOpen = false;
        private bool isOpening = false;
        private EmeraldItemSystem itemSystem;
        
        private void Awake()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
                
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
                
            // Find the item system
            itemSystem = FindObjectOfType<EmeraldItemSystem>();
        }
        
        /// <summary>
        /// Attempt to open the chest
        /// </summary>
        public void OpenChest()
        {
            if (isOpen || isOpening) return;
            
            if (isLocked)
            {
                Debug.Log($"Chest is locked. Required key: {requiredKeyID}");
                PlaySound(lockedSound);
                onLocked?.Invoke();
                return;
            }
            
            isOpening = true;
            PlaySound(openSound);
            
            // Play open animation
            if (animator != null)
            {
                animator.SetTrigger(openAnimationTrigger);
            }
            else if (lidObject != null)
            {
                // Simple lid rotation
                StartCoroutine(OpenLid());
            }
            
            // Spawn particles
            if (openParticles != null)
            {
                openParticles.Play();
            }
            
            // Spawn loot after a delay
            Invoke(nameof(SpawnLoot), 0.5f);
            
            onOpen?.Invoke();
        }
        
        private System.Collections.IEnumerator OpenLid()
        {
            Quaternion startRotation = lidObject.transform.localRotation;
            Quaternion targetRotation = Quaternion.Euler(lidOpenAngle, 0, 0);
            
            float elapsed = 0f;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * lidOpenSpeed;
                lidObject.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsed);
                yield return null;
            }
            
            isOpen = true;
            isOpening = false;
        }
        
        private void SpawnLoot()
        {
            isOpen = true;
            isOpening = false;
            
            // Spawn gold
            int goldAmount = Random.Range(goldMin, goldMax + 1);
            if (goldAmount > 0)
            {
                Debug.Log($"Chest granted {goldAmount} gold!");
                onGoldLooted?.Invoke(goldAmount);
                
                // Add gold to player inventory if item system exists
                if (itemSystem != null)
                {
                    itemSystem.AddGold(goldAmount);
                }
            }
            
            // Spawn items from loot table
            foreach (var lootItem in lootTable)
            {
                if (Random.value <= lootItem.dropChance)
                {
                    int quantity = Random.Range(lootItem.minQuantity, lootItem.maxQuantity + 1);
                    
                    for (int i = 0; i < quantity; i++)
                    {
                        SpawnItem(lootItem.itemPrefab);
                    }
                }
            }
            
            // Destroy chest if configured
            if (destroyOnEmpty)
            {
                Destroy(gameObject, 2f);
            }
        }
        
        private void SpawnItem(GameObject itemPrefab)
        {
            if (itemPrefab == null) return;
            
            // Calculate spawn position
            Vector2 randomCircle = Random.insideUnitCircle * lootSpawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, lootSpawnHeight, randomCircle.y);
            
            // Spawn the item
            GameObject spawnedItem = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
            
            // Add some physics force for effect
            Rigidbody rb = spawnedItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 force = new Vector3(randomCircle.x, 2f, randomCircle.y) * 2f;
                rb.AddForce(force, ForceMode.Impulse);
            }
            
            Debug.Log($"Chest spawned item: {itemPrefab.name}");
            onItemLooted?.Invoke(spawnedItem);
        }
        
        /// <summary>
        /// Unlock the chest with a key
        /// </summary>
        public bool UnlockWithKey(string keyID)
        {
            if (isLocked && keyID == requiredKeyID)
            {
                isLocked = false;
                Debug.Log($"Chest unlocked with key: {keyID}");
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Force lock/unlock the chest
        /// </summary>
        public void SetLocked(bool locked)
        {
            isLocked = locked;
        }
        
        /// <summary>
        /// Check if the chest can be opened
        /// </summary>
        public bool CanOpen()
        {
            return !isOpen && !isOpening && !isLocked;
        }
        
        private void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
        
        /// <summary>
        /// Handle interaction from XR controllers
        /// </summary>
        public void OnInteract()
        {
            OpenChest();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // Optional: Auto-open when hero approaches
            if (other.CompareTag("Player") && !isLocked)
            {
                // You can enable this if you want chests to open automatically
                // OpenChest();
            }
        }
    }
}