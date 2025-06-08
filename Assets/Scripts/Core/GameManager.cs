using UnityEngine;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// Central orchestrator for the entire game experience.
/// Manages game state transitions, coordinates MR/VR systems, handles save/load, and integrates with VRIF.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Game state enum
    public enum GameState { Menu, Calibration, Gameplay, Paused }
    public GameState CurrentState { get; private set; }

    // Events
    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;

    // VRIF References
    [Header("VRIF Integration")]
    [SerializeField] private GameObject vrPlayerPrefab;
    private GameObject currentVRPlayer;

    #region Unity Methods
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Don't auto-switch state here - let StartupManager handle it
        // SwitchState(GameState.Menu);
    }

    private void OnEnable()
    {
        // Subscribe to startup complete event if StartupManager exists
        StartupManager.OnStartupComplete += OnStartupComplete;
    }

    private void OnDisable()
    {
        StartupManager.OnStartupComplete -= OnStartupComplete;
    }
    #endregion

    #region Game State Management
    public void SwitchState(GameState newState)
    {
        if (CurrentState == newState) return;
        
        // Exit current state
        ExitState(CurrentState);
        
        // Enter new state
        GameState previousState = CurrentState;
        CurrentState = newState;
        EnterState(newState, previousState);
        
        OnGameStateChanged?.Invoke(newState);
    }

    private void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                // Clean up menu UI
                if (VRIFUIManager.Instance != null)
                    VRIFUIManager.Instance.HideMenu();
                break;
                
            case GameState.Calibration:
                // Save calibration data
                if (MRController.Instance != null)
                    SaveSystem.Instance?.SaveCalibrationData();
                break;
                
            case GameState.Gameplay:
                // Stop gameplay systems
                if (EdgarDungeonController.Instance != null)
                    EdgarDungeonController.Instance.enabled = false;
                if (EmeraldCombatManager.Instance != null)
                    EmeraldCombatManager.Instance.enabled = false;
                break;
                
            case GameState.Paused:
                // Resume time
                Time.timeScale = 1f;
                OnGameResumed?.Invoke();
                break;
        }
    }

    private void EnterState(GameState state, GameState previousState)
    {
        switch (state)
        {
            case GameState.Menu:
                // Show menu UI
                if (VRIFUIManager.Instance != null)
                    VRIFUIManager.Instance.ShowMenu();
                // Disable player movement
                DisablePlayerControls();
                break;
                
            case GameState.Calibration:
                // Start MR calibration
                if (MRController.Instance != null)
                {
                    MRController.Instance.SetMode(MRController.MRMode.MR);
                    if (VRIFUIManager.Instance != null)
                        VRIFUIManager.Instance.ShowCalibrationUI();
                }
                break;
                
            case GameState.Gameplay:
                // Start/Resume gameplay
                if (previousState == GameState.Menu)
                {
                    // New game - generate dungeon
                    if (EdgarDungeonController.Instance != null)
                    {
                        EdgarDungeonController.Instance.enabled = true;
                        EdgarDungeonController.Instance.GenerateDungeon();
                    }
                    // Spawn player if needed
                    SpawnVRPlayer();
                    // Spawn hero
                    if (MetaAvatarHero.Instance != null)
                        MetaAvatarHero.Instance.SpawnHero();
                }
                
                // Enable systems
                if (EmeraldCombatManager.Instance != null)
                    EmeraldCombatManager.Instance.enabled = true;
                if (VRIFVoiceController.Instance != null)
                    VRIFVoiceController.Instance.StartListening();
                    
                EnablePlayerControls();
                break;
                
            case GameState.Paused:
                // Pause gameplay
                Time.timeScale = 0f;
                if (VRIFUIManager.Instance != null)
                    VRIFUIManager.Instance.ShowPauseMenu();
                OnGamePaused?.Invoke();
                break;
        }
    }
    #endregion

    #region Save/Load
    public void SaveGame()
    {
        if (SaveSystem.Instance == null) return;
        
        // Create save data
        SaveData saveData = new SaveData
        {
            currentState = CurrentState,
            dungeonSeed = EdgarDungeonController.Instance?.CurrentSeed ?? 0,
            heroPosition = MetaAvatarHero.Instance?.transform.position ?? Vector3.zero,
            heroHealth = EmeraldHeroAI.Instance?.GetCurrentHealth() ?? 100,
            playerPosition = currentVRPlayer?.transform.position ?? Vector3.zero
        };
        
        SaveSystem.Instance.SaveGame(saveData);
        Debug.Log("[GameManager] Game saved successfully");
    }
    
    public void LoadGame()
    {
        if (SaveSystem.Instance == null) return;
        
        SaveData saveData = SaveSystem.Instance.LoadGame();
        if (saveData != null)
        {
            // Restore game state
            if (EdgarDungeonController.Instance != null && saveData.dungeonSeed != 0)
            {
                EdgarDungeonController.Instance.SetSeed(saveData.dungeonSeed);
            }
            
            if (MetaAvatarHero.Instance != null && saveData.heroPosition != Vector3.zero)
            {
                MetaAvatarHero.Instance.transform.position = saveData.heroPosition;
            }
            
            if (currentVRPlayer != null && saveData.playerPosition != Vector3.zero)
            {
                currentVRPlayer.transform.position = saveData.playerPosition;
            }
            
            Debug.Log("[GameManager] Game loaded successfully");
        }
    }
    #endregion

    #region VRIF Integration
    private void SpawnVRPlayer()
    {
        if (currentVRPlayer == null && vrPlayerPrefab != null)
        {
            currentVRPlayer = Instantiate(vrPlayerPrefab);
            currentVRPlayer.name = "VR Player";
            
            // Configure VRIF player settings
            var player = currentVRPlayer.GetComponent<BNG.Player>();
            if (player != null)
            {
                // Set up player preferences
                player.SnapTurnAmount = 45f;
                player.SmoothTurnSpeed = 90f;
            }
        }
    }

    private void EnablePlayerControls()
    {
        if (currentVRPlayer != null)
        {
            var locomotion = currentVRPlayer.GetComponentInChildren<BNG.PlayerTeleport>();
            if (locomotion != null) locomotion.enabled = true;
            
            var smoothLoco = currentVRPlayer.GetComponentInChildren<BNG.SmoothLocomotion>();
            if (smoothLoco != null) smoothLoco.enabled = true;
        }
    }

    private void DisablePlayerControls()
    {
        if (currentVRPlayer != null)
        {
            var locomotion = currentVRPlayer.GetComponentInChildren<BNG.PlayerTeleport>();
            if (locomotion != null) locomotion.enabled = false;
            
            var smoothLoco = currentVRPlayer.GetComponentInChildren<BNG.SmoothLocomotion>();
            if (smoothLoco != null) smoothLoco.enabled = false;
        }
    }

    public GameObject GetVRPlayer()
    {
        return currentVRPlayer;
    }
    #endregion

    #region Public Methods
    public void PauseGame()
    {
        if (CurrentState == GameState.Gameplay)
        {
            SwitchState(GameState.Paused);
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            SwitchState(GameState.Gameplay);
        }
    }

    public void StartNewGame()
    {
        SaveGame(); // Save current progress if any
        SwitchState(GameState.Gameplay);
    }

    public void ReturnToMenu()
    {
        SaveGame();
        SwitchState(GameState.Menu);
    }

    private void OnStartupComplete()
    {
        Debug.Log("[GameManager] Startup complete, ready for gameplay");
    }
    #endregion
}

// Save data structure
[System.Serializable]
public class SaveData
{
    public GameManager.GameState currentState;
    public int dungeonSeed;
    public Vector3 heroPosition;
    public float heroHealth;
    public Vector3 playerPosition;
    public string timestamp;

    public SaveData()
    {
        timestamp = System.DateTime.Now.ToString();
    }
} 