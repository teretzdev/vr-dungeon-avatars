using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// Wraps Edgar Pro room data for game-specific functionality.
/// Manages enemy spawns, treasures, and room events.
/// </summary>
public class EdgarRoomWrapper : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private string roomID = "Room_01";
    [SerializeField] private bool requiresClearingEnemies = true;
    [SerializeField] private bool autoSpawnOnEnter = true;
    
    [Header("Spawn Points")]
    public Transform[] enemySpawnPoints;
    public Transform[] treasureSpawnPoints;
    
    [Header("Spawn Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject[] treasurePrefabs;
    [SerializeField] private int enemyCount = 3;
    [SerializeField] private int treasureCount = 1;
    
    [Header("Events")]
    public UnityEvent onRoomEntered = new UnityEvent();
    public UnityEvent onRoomCompleted = new UnityEvent();
    public UnityEvent onAllEnemiesDefeated = new UnityEvent();
    
    // Internal events
    public event Action OnRoomCompleted;
    public event Action<int> OnEnemyDefeated;
    
    // Room state
    public bool isCompleted = false;
    private bool hasBeenEntered = false;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private int enemiesDefeated = 0;

    private void Start()
    {
        // Register with dungeon controller if available
        var dungeonController = FindObjectOfType<EdgarDungeonController>();
        if (dungeonController != null)
        {
            // TODO: Register this room with the dungeon controller
        }
    }

    /// <summary>
    /// Called when the player enters this room
    /// </summary>
    public void OnRoomEntered()
    {
        if (!hasBeenEntered)
        {
            hasBeenEntered = true;
            Debug.Log($"Entered room: {roomID}");
            
            onRoomEntered?.Invoke();
            
            if (autoSpawnOnEnter)
            {
                SpawnAllContent();
            }
        }
    }

    /// <summary>
    /// Spawn all room content (enemies and treasures)
    /// </summary>
    public void SpawnAllContent()
    {
        if (enemyPrefabs.Length > 0)
        {
            SpawnEnemies(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)], enemyCount);
        }
        
        if (treasurePrefabs.Length > 0 && !requiresClearingEnemies)
        {
            SpawnTreasures(treasurePrefabs[UnityEngine.Random.Range(0, treasurePrefabs.Length)], treasureCount);
        }
    }

    public void SpawnEnemies(GameObject enemyPrefab, int count)
    {
        if (enemyPrefab == null) return;
        
        for (int i = 0; i < count && i < enemySpawnPoints.Length; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, enemySpawnPoints[i].position, Quaternion.identity);
            enemy.transform.SetParent(transform);
            spawnedEnemies.Add(enemy);
            
            // Subscribe to enemy death event if it has one
            var enemyController = enemy.GetComponent<VRIFEnemyController>();
            if (enemyController != null)
            {
                // TODO: Subscribe to enemy death event when implemented
            }
        }
        
        Debug.Log($"Spawned {count} enemies in room {roomID}");
    }

    public void SpawnTreasures(GameObject treasurePrefab, int count)
    {
        if (treasurePrefab == null) return;
        
        for (int i = 0; i < count && i < treasureSpawnPoints.Length; i++)
        {
            GameObject treasure = Instantiate(treasurePrefab, treasureSpawnPoints[i].position, Quaternion.identity);
            treasure.transform.SetParent(transform);
        }
        
        Debug.Log($"Spawned {count} treasures in room {roomID}");
    }

    /// <summary>
    /// Called when an enemy in this room is defeated
    /// </summary>
    public void OnEnemyDefeatedInRoom(GameObject enemy)
    {
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);
            enemiesDefeated++;
            OnEnemyDefeated?.Invoke(enemiesDefeated);
            
            Debug.Log($"Enemy defeated in room {roomID}. {spawnedEnemies.Count} remaining.");
            
            // Check if all enemies are defeated
            if (spawnedEnemies.Count == 0)
            {
                onAllEnemiesDefeated?.Invoke();
                
                // Spawn treasures after clearing enemies if required
                if (requiresClearingEnemies && treasurePrefabs.Length > 0)
                {
                    SpawnTreasures(treasurePrefabs[UnityEngine.Random.Range(0, treasurePrefabs.Length)], treasureCount);
                }
                
                // Auto-complete room if it requires clearing enemies
                if (requiresClearingEnemies)
                {
                    CompleteRoom();
                }
            }
        }
    }

    /// <summary>
    /// Mark this room as completed
    /// </summary>
    public void CompleteRoom()
    {
        if (!isCompleted)
        {
            isCompleted = true;
            OnRoomCompleted?.Invoke();
            onRoomCompleted?.Invoke();
            
            Debug.Log($"Room {roomID} completed!");
            
            // Notify GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnRoomCompleted(roomID);
            }
        }
    }

    /// <summary>
    /// Force clear all enemies in the room
    /// </summary>
    public void ClearAllEnemies()
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        spawnedEnemies.Clear();
        onAllEnemiesDefeated?.Invoke();
    }

    /// <summary>
    /// Get room information
    /// </summary>
    public string GetRoomID() => roomID;
    public bool IsCompleted() => isCompleted;
    public int GetRemainingEnemies() => spawnedEnemies.Count;
    public bool HasBeenEntered() => hasBeenEntered;

    #region Room Logic
    // Additional room-specific logic can be added here
    #endregion
} 