using UnityEngine;
using UnityEngine.Events;
using Edgar.Unity;
using System.Collections.Generic;

/// <summary>
/// Enhances Edgar-generated rooms with VR-specific features.
/// Manages enemy spawning, treasure placement, and room completion triggers.
/// </summary>
public class EdgarRoomWrapper : MonoBehaviour
{
    [Header("Room Configuration")]
    [SerializeField] private int minEnemies = 1;
    [SerializeField] private int maxEnemies = 3;
    [SerializeField] private float treasureChance = 0.3f;
    
    [Header("Spawn Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject[] treasurePrefabs;
    [SerializeField] private GameObject[] itemPrefabs;
    
    [Header("Spawn Points")]
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private Transform[] treasureSpawnPoints;
    
    [Header("Events")]
    public UnityEvent OnRoomEntered;
    public UnityEvent OnRoomCleared;
    public UnityEvent OnTreasureOpened;
    
    private RoomInstance roomData;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool isCleared = false;
    private bool hasBeenEntered = false;

    public void Initialize(RoomInstance room)
    {
        roomData = room;
        
        // Find spawn points in the room
        FindSpawnPoints();
        
        // Set up room trigger
        SetupRoomTrigger();
        
        // Spawn initial content
        SpawnRoomContent();
    }

    private void FindSpawnPoints()
    {
        // Look for spawn point markers in the room
        var enemyPoints = new List<Transform>();
        var treasurePoints = new List<Transform>();
        
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("EnemySpawnPoint"))
                enemyPoints.Add(child);
            else if (child.CompareTag("TreasureSpawnPoint"))
                treasurePoints.Add(child);
        }
        
        enemySpawnPoints = enemyPoints.ToArray();
        treasureSpawnPoints = treasurePoints.ToArray();
        
        // If no spawn points found, create default positions
        if (enemySpawnPoints.Length == 0)
        {
            CreateDefaultSpawnPoints();
        }
    }

    private void CreateDefaultSpawnPoints()
    {
        var bounds = GetRoomBounds();
        var spawnPoints = new List<Transform>();
        
        // Create 4 spawn points at corners
        Vector3[] corners = new Vector3[]
        {
            new Vector3(bounds.min.x + 1, bounds.center.y, bounds.min.z + 1),
            new Vector3(bounds.max.x - 1, bounds.center.y, bounds.min.z + 1),
            new Vector3(bounds.min.x + 1, bounds.center.y, bounds.max.z - 1),
            new Vector3(bounds.max.x - 1, bounds.center.y, bounds.max.z - 1)
        };
        
        foreach (var corner in corners)
        {
            GameObject spawnPoint = new GameObject("EnemySpawnPoint");
            spawnPoint.transform.position = corner;
            spawnPoint.transform.parent = transform;
            spawnPoints.Add(spawnPoint.transform);
        }
        
        enemySpawnPoints = spawnPoints.ToArray();
        
        // Create treasure spawn point at center
        GameObject treasurePoint = new GameObject("TreasureSpawnPoint");
        treasurePoint.transform.position = bounds.center;
        treasurePoint.transform.parent = transform;
        treasureSpawnPoints = new Transform[] { treasurePoint.transform };
    }

    private Bounds GetRoomBounds()
    {
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }

    private void SetupRoomTrigger()
    {
        // Add or find room entrance trigger
        BoxCollider trigger = GetComponent<BoxCollider>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            
            // Set trigger bounds to room bounds
            var bounds = GetRoomBounds();
            trigger.center = transform.InverseTransformPoint(bounds.center);
            trigger.size = bounds.size;
        }
    }

    private void SpawnRoomContent()
    {
        // Don't spawn in the starting room
        if (roomData != null && roomData.Room.GetRoomTemplateConfig().Name.Contains("Start"))
            return;
            
        // Spawn enemies
        SpawnEnemies();
        
        // Spawn treasure with chance
        if (Random.value < treasureChance)
        {
            SpawnTreasure();
        }
    }

    private void SpawnEnemies()
    {
        if (enemyPrefabs.Length == 0 || enemySpawnPoints.Length == 0) return;
        
        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
        enemyCount = Mathf.Min(enemyCount, enemySpawnPoints.Length);
        
        // Shuffle spawn points
        List<Transform> availablePoints = new List<Transform>(enemySpawnPoints);
        
        for (int i = 0; i < enemyCount; i++)
        {
            if (availablePoints.Count == 0) break;
            
            // Pick random spawn point
            int pointIndex = Random.Range(0, availablePoints.Count);
            Transform spawnPoint = availablePoints[pointIndex];
            availablePoints.RemoveAt(pointIndex);
            
            // Pick random enemy
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            
            // Spawn enemy
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            enemy.transform.parent = transform;
            spawnedEnemies.Add(enemy);
            
            // Subscribe to enemy death event
            var emeraldAI = enemy.GetComponent<EmeraldAI.EmeraldSystem>();
            if (emeraldAI != null)
            {
                emeraldAI.CombatEvents.OnDeath.AddListener(() => OnEnemyDefeated(enemy));
            }
        }
    }

    private void SpawnTreasure()
    {
        if (treasurePrefabs.Length == 0 || treasureSpawnPoints.Length == 0) return;
        
        Transform spawnPoint = treasureSpawnPoints[Random.Range(0, treasureSpawnPoints.Length)];
        GameObject treasurePrefab = treasurePrefabs[Random.Range(0, treasurePrefabs.Length)];
        
        GameObject treasure = Instantiate(treasurePrefab, spawnPoint.position, spawnPoint.rotation);
        treasure.transform.parent = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if player entered
        if (other.CompareTag("Player") && !hasBeenEntered)
        {
            hasBeenEntered = true;
            OnRoomEntered?.Invoke();
            
            // Activate enemies
            foreach (var enemy in spawnedEnemies)
            {
                if (enemy != null)
                {
                    var emeraldAI = enemy.GetComponent<EmeraldAI.EmeraldSystem>();
                    if (emeraldAI != null)
                    {
                        emeraldAI.DetectionComponent.enabled = true;
                    }
                }
            }
            
            Debug.Log($"[EdgarRoomWrapper] Room entered: {gameObject.name}");
        }
    }

    private void OnEnemyDefeated(GameObject enemy)
    {
        spawnedEnemies.Remove(enemy);
        
        // Check if room is cleared
        if (spawnedEnemies.Count == 0 && !isCleared)
        {
            isCleared = true;
            OnRoomCleared?.Invoke();
            
            // Spawn reward items
            SpawnRewardItems();
            
            Debug.Log($"[EdgarRoomWrapper] Room cleared: {gameObject.name}");
        }
    }

    private void SpawnRewardItems()
    {
        if (itemPrefabs.Length == 0) return;
        
        // Spawn 1-3 items at random positions
        int itemCount = Random.Range(1, 4);
        var bounds = GetRoomBounds();
        
        for (int i = 0; i < itemCount; i++)
        {
            GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            Vector3 randomPos = new Vector3(
                Random.Range(bounds.min.x + 1, bounds.max.x - 1),
                bounds.center.y,
                Random.Range(bounds.min.z + 1, bounds.max.z - 1)
            );
            
            Instantiate(itemPrefab, randomPos, Quaternion.identity, transform);
        }
    }

    public bool IsCleared()
    {
        return isCleared;
    }

    public int GetRemainingEnemies()
    {
        return spawnedEnemies.Count;
    }
} 