using UnityEngine;
using System;

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
    public static event Action<string> OnRoomEnteredEvent;
    public static event Action<string> OnRoomExitedEvent;
    public static event Action OnDungeonCompleted;

    // Dungeon progress tracking
    private int roomsCompleted = 0;
    private string currentRoomID = "";

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
        SwitchState(GameState.Menu);
    }
    #endregion

    #region Game State Management
    public void SwitchState(GameState newState)
    {
        if (CurrentState == newState) return;
        
        // Exit current state
        OnStateExit(CurrentState);
        
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
        
        // Enter new state
        OnStateEnter(newState);
    }
    
    private void OnStateEnter(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                Time.timeScale = 1f;
                break;
            case GameState.Calibration:
                // TODO: Start MR calibration
                break;
            case GameState.Gameplay:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
        }
    }
    
    private void OnStateExit(GameState state)
    {
        // Cleanup when exiting states
    }
    #endregion

    #region Dungeon Progress
    public void OnRoomEntered(string roomID)
    {
        currentRoomID = roomID;
        Debug.Log($"GameManager: Entered room {roomID}");
        OnRoomEnteredEvent?.Invoke(roomID);
    }
    
    public void OnRoomExited(string roomID)
    {
        Debug.Log($"GameManager: Exited room {roomID}");
        OnRoomExitedEvent?.Invoke(roomID);
    }
    
    public void OnRoomCompleted(string roomID)
    {
        roomsCompleted++;
        Debug.Log($"GameManager: Completed room {roomID}. Total rooms completed: {roomsCompleted}");
    }
    
    public void OnDungeonComplete()
    {
        Debug.Log("GameManager: Dungeon completed!");
        OnDungeonCompleted?.Invoke();
        // TODO: Show completion UI, rewards, etc.
    }
    #endregion

    #region Save/Load
    public void SaveGame()
    {
        PlayerPrefs.SetInt("RoomsCompleted", roomsCompleted);
        PlayerPrefs.SetString("CurrentRoomID", currentRoomID);
        PlayerPrefs.SetString("CurrentGameState", CurrentState.ToString());
        PlayerPrefs.Save();
        Debug.Log("Game saved!");
    }
    
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("RoomsCompleted"))
        {
            roomsCompleted = PlayerPrefs.GetInt("RoomsCompleted");
            currentRoomID = PlayerPrefs.GetString("CurrentRoomID", "");
            string savedState = PlayerPrefs.GetString("CurrentGameState", "Menu");
            
            if (Enum.TryParse<GameState>(savedState, out GameState state))
            {
                SwitchState(state);
            }
            
            Debug.Log($"Game loaded! Rooms completed: {roomsCompleted}");
        }
    }
    #endregion

    #region VRIF Integration
    // TODO: Reference VRIF systems and manage VR-specific logic
    #endregion
    
    #region Public Accessors
    public int GetRoomsCompleted() => roomsCompleted;
    public string GetCurrentRoomID() => currentRoomID;
    #endregion
} 