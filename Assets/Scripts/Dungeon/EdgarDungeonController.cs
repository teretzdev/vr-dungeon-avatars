using UnityEngine;
using System;
using Edgar.Unity;

/// <summary>
/// Wrapper for Edgar Pro dungeon generation system.
/// Interfaces with Edgar Pro for procedural layout and VR optimization.
/// </summary>
public class EdgarDungeonController : MonoBehaviour
{
    public static EdgarDungeonController Instance { get; private set; }

    public event Action OnDungeonGenerated;
    public event Action<float> OnGenerationProgress;
    
    [Header("Edgar Pro Configuration")]
    [SerializeField] private DungeonGeneratorGrid3D edgarGenerator;
    [SerializeField] private int maxRooms = 10;
    [SerializeField] private bool useRandomSeed = true;
    
    // Seed management
    private string currentSeed;
    public int CurrentSeed { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Find Edgar generator if not assigned
        if (edgarGenerator == null)
        {
            edgarGenerator = GetComponent<DungeonGeneratorGrid3D>();
        }
    }

    public void GenerateDungeon(string seed = null)
    {
        // Convert string seed to int for Edgar Pro
        if (string.IsNullOrEmpty(seed))
        {
            CurrentSeed = useRandomSeed ? UnityEngine.Random.Range(0, int.MaxValue) : 0;
            currentSeed = CurrentSeed.ToString();
        }
        else
        {
            currentSeed = seed;
            CurrentSeed = seed.GetHashCode();
        }

        // Configure and generate with Edgar Pro
        if (edgarGenerator != null)
        {
            // Set the seed
            edgarGenerator.RandomGeneratorSeed = CurrentSeed;
            
            // Subscribe to events
            edgarGenerator.OnGenerationComplete += OnEdgarGenerationComplete;
            
            // Start generation
            edgarGenerator.Generate();
            
            Debug.Log($"[EdgarDungeonController] Starting dungeon generation with seed: {CurrentSeed}");
        }
        else
        {
            Debug.LogError("[EdgarDungeonController] Edgar generator not found!");
        }
    }

    public void SetSeed(int seed)
    {
        CurrentSeed = seed;
        currentSeed = seed.ToString();
        
        if (edgarGenerator != null)
        {
            edgarGenerator.RandomGeneratorSeed = seed;
        }
    }

    private void OnEdgarGenerationComplete(GeneratedLevel level)
    {
        Debug.Log($"[EdgarDungeonController] Dungeon generated successfully with {level.Rooms.Count} rooms");
        
        // Process generated rooms
        foreach (var room in level.Rooms)
        {
            // Add EdgarRoomWrapper to each room for enemy/loot spawning
            var roomWrapper = room.RoomInstance.AddComponent<EdgarRoomWrapper>();
            roomWrapper.Initialize(room);
        }
        
        // Notify other systems
        OnDungeonGenerated?.Invoke();
        
        // Apply dungeon scaling for MR if needed
        if (MRController.Instance != null && MRController.Instance.CurrentMode == MRController.MRMode.MR)
        {
            var scaler = GetComponent<DungeonScaler>();
            if (scaler != null)
            {
                scaler.ScaleDungeonToMRSpace(level.RootGameObject);
            }
        }
    }

    public void RegenerateDungeon()
    {
        // Clear existing dungeon
        if (edgarGenerator != null)
        {
            var existingDungeon = GameObject.Find("Generated Level");
            if (existingDungeon != null)
            {
                Destroy(existingDungeon);
            }
        }
        
        // Generate new dungeon with same seed
        GenerateDungeon(currentSeed);
    }

    private void OnDestroy()
    {
        if (edgarGenerator != null)
        {
            edgarGenerator.OnGenerationComplete -= OnEdgarGenerationComplete;
        }
    }
} 