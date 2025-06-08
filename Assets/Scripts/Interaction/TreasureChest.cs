using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

namespace DungeonYou.Interaction
{
    /// <summary>
    /// Interactive treasure chest that can be opened to grant loot.
    /// Supports animations, loot tables, and integration with the item system.
    /// </summary>
    public class TreasureChest : MonoBehaviour
    {
        [System.Serializable]
        public class LootItem
        {
            public GameObject itemPrefab;
            public int minQuantity = 1;
            public int maxQuantity = 1;
            public float dropChance = 1f; // 0-1 probability
        }
        
        [Header("Chest Configuration")]
        [SerializeField] private bool isLocked = false;
        [SerializeField] private string requiredKey = "";
        [SerializeField] private bool isOpened = false;
        [SerializeField] private bool canBeOpenedMultipleTimes = false;
        
        [Header("Loot Table")]
        [SerializeField] private List<LootItem> lootTable = new List<LootItem>();
        [SerializeField] private int guaranteedGold = 10;
        [SerializeField] private int randomGoldMax = 50;
        
        [Header("Animation")]
        [SerializeField] private Animator chestAnimator;
        [SerializeField] private string openAnimationTrigger = "Open";
        [SerializeField] private float lootSpawnDelay = 0.5f;
        [SerializeField] private Transform lootSpawnPoint;
        
        [Header("Audio")]
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip lockedSound;
        [SerializeField] private AudioClip lootSound;
        
        [Header("Effects")]
        [SerializeField] private GameObject openEffect;
        [SerializeField] private float effectDuration = 2f;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onChestOpened;
        [SerializeField] private UnityEvent onChestLocked;
        [SerializeField] private UnityEvent<int> onGoldLooted; // Passes gold amount
        [SerializeField] private UnityEvent<GameObject> onItemLooted; // Passes item prefab
        
        private AudioSource audioSource;
        private bool isInteracting = false;
        
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
                
            if (lootSpawnPoint == null)
                lootSpawnPoint = transform;
                
            if (chestAnimator == null)
                chestAnimator = GetComponent<Animator>();
        }
        
        public void Interact()
        {
            if (isInteracting) return;
            
            if (isOpened && !canBeOpenedMultipleTimes)
                return;
                
            if (isLocked)
            {
                PlaySound(lockedSound);
                onChestLocked?.Invoke();
                return;
            }
            
            StartCoroutine(OpenChest());
        }
        
        public void Unlock(string key = "")
        {
            if (string.IsNullOrEmpty(requiredKey) || requiredKey == key)
            {
                isLocked = false;
            }
        }
        
        private IEnumerator OpenChest()
        {
            isInteracting = true;
            isOpened = true;
            
            // Play open animation
            if (chestAnimator != null)
                chestAnimator.SetTrigger(openAnimationTrigger);
                
            PlaySound(openSound);
            onChestOpened?.Invoke();
            
            // Spawn effect
            if (openEffect != null)
            {
                var effect = Instantiate(openEffect, lootSpawnPoint.position, Quaternion.identity);
                Destroy(effect, effectDuration);
            }
            
            // Wait before spawning loot
            yield return new WaitForSeconds(lootSpawnDelay);
            
            // Spawn loot
            SpawnLoot();
            
            isInteracting = false;
        }
        
        private void SpawnLoot()
        {
            // Spawn gold
            int goldAmount = guaranteedGold + Random.Range(0, randomGoldMax);
            if (goldAmount > 0)
            {
                onGoldLooted?.Invoke(goldAmount);
                
                // Add gold to hero's inventory if available
                var itemSystem = FindObjectOfType<Combat.EmeraldItemSystem>();
                if (itemSystem != null)
                    itemSystem.AddGold(goldAmount);
            }
            
            // Spawn items from loot table
            foreach (var lootItem in lootTable)
            {
                if (Random.value <= lootItem.dropChance)
                {
                    int quantity = Random.Range(lootItem.minQuantity, lootItem.maxQuantity + 1);
                    
                    for (int i = 0; i < quantity; i++)
                    {
                        SpawnLootItem(lootItem.itemPrefab);
                    }
                }
            }
            
            PlaySound(lootSound);
        }
        
        private void SpawnLootItem(GameObject itemPrefab)
        {
            if (itemPrefab == null) return;
            
            // Calculate spawn position with some randomness
            Vector3 spawnOffset = new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(0.5f, 1f),
                Random.Range(-0.5f, 0.5f)
            );
            
            Vector3 spawnPosition = lootSpawnPoint.position + spawnOffset;
            GameObject lootItem = Instantiate(itemPrefab, spawnPosition, Random.rotation);
            
            // Add some physics force for effect
            var rb = lootItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 force = spawnOffset.normalized * Random.Range(2f, 5f);
                rb.AddForce(force, ForceMode.Impulse);
            }
            
            onItemLooted?.Invoke(lootItem);
        }
        
        private void PlaySound(AudioClip clip)
        {
            if (clip != null && audioSource != null)
                audioSource.PlayOneShot(clip);
        }
        
        // Save/Load support
        public void SaveState()
        {
            PlayerPrefs.SetInt($"Chest_{gameObject.name}_Opened", isOpened ? 1 : 0);
        }
        
        public void LoadState()
        {
            isOpened = PlayerPrefs.GetInt($"Chest_{gameObject.name}_Opened", 0) == 1;
        }
        
        public bool IsOpened() => isOpened;
        public bool IsLocked() => isLocked;
    }
}