using UnityEngine;
using System;

/// <summary>
/// Wrapper for Edgar Pro dungeon generation system.
/// Interfaces with Edgar Pro for procedural layout and VR optimization.
/// </summary>
public class EdgarDungeonController : MonoBehaviour
{
    public static EdgarDungeonController Instance { get; private set; }

    public event Action OnDungeonGenerated;
    public string currentSeed;

    // Reference to Edgar Pro generator (replace with actual Edgar Pro type)
    public MonoBehaviour edgarGenerator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void GenerateDungeon(string seed = null)
    {
        currentSeed = seed ?? Guid.NewGuid().ToString();
        // Example: Configure Edgar Pro generator
        // edgarGenerator.SetSeed(currentSeed);
        // edgarGenerator.Generate();
        Debug.Log($"Dungeon generated with seed: {currentSeed}");
        OnDungeonGenerated?.Invoke();
    }
} 