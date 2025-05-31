using UnityEngine;
using System;

/// <summary>
/// Wraps Edgar Pro room data for game-specific functionality.
/// Manages enemy spawns, treasures, and room events.
/// </summary>
public class EdgarRoomWrapper : MonoBehaviour
{
    public event Action OnRoomCompleted;
    public Transform[] enemySpawnPoints;
    public Transform[] treasureSpawnPoints;
    public bool isCompleted = false;

    public void SpawnEnemies(GameObject enemyPrefab, int count)
    {
        for (int i = 0; i < count && i < enemySpawnPoints.Length; i++)
        {
            Instantiate(enemyPrefab, enemySpawnPoints[i].position, Quaternion.identity);
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

    #region Room Logic
    // TODO: Implement room-specific logic and events
    #endregion
} 