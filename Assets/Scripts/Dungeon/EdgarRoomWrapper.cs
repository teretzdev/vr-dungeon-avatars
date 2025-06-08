using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Wraps Edgar Pro room data for game-specific functionality.
/// Manages enemy spawns, treasures, and room events.
/// </summary>
public class EdgarRoomWrapper : MonoBehaviour
{
    public event Action OnRoomCompleted;
    public event Action OnHeroEntered;
    public Transform[] enemySpawnPoints;
    public Transform[] treasureSpawnPoints;
    public bool isCompleted = false;
    
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool heroHasEntered = false;

    public void SpawnEnemies(GameObject enemyPrefab, int count)
    {
        for (int i = 0; i < count && i < enemySpawnPoints.Length; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, enemySpawnPoints[i].position, Quaternion.identity);
            spawnedEnemies.Add(enemy);
        }
    }

    public void SpawnTreasures(GameObject treasurePrefab, int count)
    {
        for (int i = 0; i < count && i < treasureSpawnPoints.Length; i++)
        {
            Instantiate(treasurePrefab, treasureSpawnPoints[i].position, Quaternion.identity);
        }
    }

    public void CompleteRoom()
    {
        isCompleted = true;
        OnRoomCompleted?.Invoke();
        Debug.Log($"Room {gameObject.name} completed!");
    }
    
    public void OnHeroEntered()
    {
        if (!heroHasEntered)
        {
            heroHasEntered = true;
            OnHeroEntered?.Invoke();
            Debug.Log($"Hero entered room {gameObject.name}");
            
            // You could trigger events here like spawning enemies, starting music, etc.
        }
    }
    
    public bool IsRoomCleared()
    {
        // Check if all enemies are defeated
        spawnedEnemies.RemoveAll(enemy => enemy == null);
        
        if (spawnedEnemies.Count == 0)
        {
            if (!isCompleted)
            {
                CompleteRoom();
            }
            return true;
        }
        
        return false;
    }
    
    public void OnEnemyDefeated(GameObject enemy)
    {
        spawnedEnemies.Remove(enemy);
        
        // Check if room is cleared after enemy defeat
        if (IsRoomCleared())
        {
            Debug.Log($"All enemies defeated in room {gameObject.name}");
        }
    }

    #region Room Logic
    // TODO: Implement room-specific logic and events
    #endregion
} 