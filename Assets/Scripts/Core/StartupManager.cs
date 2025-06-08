using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Startup orchestrator that ensures all systems initialize in the correct order.
/// This should be the first script to run in the scene.
/// </summary>
public class StartupManager : MonoBehaviour
{
    [Header("Initialization Settings")]
    [SerializeField] private bool autoStartGame = true;
    [SerializeField] private float initializationDelay = 0.5f;
    
    [Header("System References")]
    [SerializeField] private GameObject gameManagerPrefab;
    [SerializeField] private GameObject mrControllerPrefab;
    [SerializeField] private GameObject networkManagerPrefab;
    [SerializeField] private GameObject saveSystemPrefab;
    [SerializeField] private GameObject performanceMonitorPrefab;
    
    [Header("Dungeon Systems")]
    [SerializeField] private GameObject dungeonControllerPrefab;
    [SerializeField] private GameObject dungeonScalerPrefab;
    [SerializeField] private GameObject dungeonRendererPrefab;
    
    [Header("Hero Systems")]
    [SerializeField] private GameObject metaAvatarHeroPrefab;
    [SerializeField] private GameObject emeraldHeroAIPrefab;
    [SerializeField] private GameObject heroVRIFControllerPrefab;
    [SerializeField] private GameObject metaAvatarAnimatorPrefab;
    
    [Header("Communication Systems")]
    [SerializeField] private GameObject voiceControllerPrefab;
    [SerializeField] private GameObject commandInterpreterPrefab;
    [SerializeField] private GameObject avatarResponseSystemPrefab;
    
    [Header("Combat Systems")]
    [SerializeField] private GameObject combatManagerPrefab;
    [SerializeField] private GameObject itemSystemPrefab;
    
    [Header("Interaction Systems")]
    [SerializeField] private GameObject xrInteractionManagerPrefab;
    [SerializeField] private GameObject uiManagerPrefab;
    
    public static event Action OnStartupComplete;
    
    private void Awake()
    {
        // Set this script to execute first
        StartCoroutine(InitializeSystems());
    }
    
    private IEnumerator InitializeSystems()
    {
        Debug.Log("[StartupManager] Beginning system initialization...");
        
        // Phase 1: Core Systems (must be first)
        yield return InitializeCoreSystem();
        
        // Phase 2: Dungeon Systems
        yield return InitializeDungeonSystems();
        
        // Phase 3: Hero Systems
        yield return InitializeHeroSystems();
        
        // Phase 4: Communication Systems
        yield return InitializeCommunicationSystems();
        
        // Phase 5: Combat Systems
        yield return InitializeCombatSystems();
        
        // Phase 6: Interaction Systems
        yield return InitializeInteractionSystems();
        
        // Phase 7: Final Setup
        yield return FinalizeStartup();
        
        Debug.Log("[StartupManager] All systems initialized successfully!");
        OnStartupComplete?.Invoke();
    }
    
    private IEnumerator InitializeCoreSystem()
    {
        Debug.Log("[StartupManager] Phase 1: Initializing Core Systems...");
        
        // GameManager must be first
        if (GameManager.Instance == null && gameManagerPrefab != null)
        {
            Instantiate(gameManagerPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        // MR Controller
        if (MRController.Instance == null && mrControllerPrefab != null)
        {
            Instantiate(mrControllerPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        // Network Manager
        if (NetworkManager.Instance == null && networkManagerPrefab != null)
        {
            Instantiate(networkManagerPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        // Save System
        if (SaveSystem.Instance == null && saveSystemPrefab != null)
        {
            Instantiate(saveSystemPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        // Performance Monitor
        if (performanceMonitorPrefab != null)
        {
            Instantiate(performanceMonitorPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(initializationDelay);
    }
    
    private IEnumerator InitializeDungeonSystems()
    {
        Debug.Log("[StartupManager] Phase 2: Initializing Dungeon Systems...");
        
        if (EdgarDungeonController.Instance == null && dungeonControllerPrefab != null)
        {
            Instantiate(dungeonControllerPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        if (dungeonScalerPrefab != null)
        {
            Instantiate(dungeonScalerPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        if (dungeonRendererPrefab != null)
        {
            Instantiate(dungeonRendererPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(initializationDelay);
    }
    
    private IEnumerator InitializeHeroSystems()
    {
        Debug.Log("[StartupManager] Phase 3: Initializing Hero Systems...");
        
        // Meta Avatar Hero must be created before its dependent systems
        if (MetaAvatarHero.Instance == null && metaAvatarHeroPrefab != null)
        {
            Instantiate(metaAvatarHeroPrefab);
            yield return new WaitForSeconds(0.2f);
        }
        
        if (MetaAvatarAnimator.Instance == null && metaAvatarAnimatorPrefab != null)
        {
            Instantiate(metaAvatarAnimatorPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        if (HeroVRIFController.Instance == null && heroVRIFControllerPrefab != null)
        {
            Instantiate(heroVRIFControllerPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        if (EmeraldHeroAI.Instance == null && emeraldHeroAIPrefab != null)
        {
            Instantiate(emeraldHeroAIPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(initializationDelay);
    }
    
    private IEnumerator InitializeCommunicationSystems()
    {
        Debug.Log("[StartupManager] Phase 4: Initializing Communication Systems...");
        
        if (VRIFVoiceController.Instance == null && voiceControllerPrefab != null)
        {
            Instantiate(voiceControllerPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        if (CommandInterpreter.Instance == null && commandInterpreterPrefab != null)
        {
            Instantiate(commandInterpreterPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        if (MetaAvatarResponseSystem.Instance == null && avatarResponseSystemPrefab != null)
        {
            Instantiate(avatarResponseSystemPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(initializationDelay);
    }
    
    private IEnumerator InitializeCombatSystems()
    {
        Debug.Log("[StartupManager] Phase 5: Initializing Combat Systems...");
        
        if (EmeraldCombatManager.Instance == null && combatManagerPrefab != null)
        {
            Instantiate(combatManagerPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        if (EmeraldItemSystem.Instance == null && itemSystemPrefab != null)
        {
            Instantiate(itemSystemPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(initializationDelay);
    }
    
    private IEnumerator InitializeInteractionSystems()
    {
        Debug.Log("[StartupManager] Phase 6: Initializing Interaction Systems...");
        
        if (xrInteractionManagerPrefab != null)
        {
            Instantiate(xrInteractionManagerPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        if (VRIFUIManager.Instance == null && uiManagerPrefab != null)
        {
            Instantiate(uiManagerPrefab);
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(initializationDelay);
    }
    
    private IEnumerator FinalizeStartup()
    {
        Debug.Log("[StartupManager] Phase 7: Finalizing Startup...");
        
        // Configure initial game state
        if (GameManager.Instance != null)
        {
            // Load saved data if exists
            if (SaveSystem.Instance != null)
            {
                GameManager.Instance.LoadGame();
            }
            
            // Switch to appropriate starting state
            if (autoStartGame)
            {
                // Check if MR calibration is needed
                if (MRController.Instance != null && MRController.Instance.CurrentMode == MRController.MRMode.MR)
                {
                    GameManager.Instance.SwitchState(GameManager.GameState.Calibration);
                }
                else
                {
                    GameManager.Instance.SwitchState(GameManager.GameState.Gameplay);
                }
            }
        }
        
        yield return new WaitForSeconds(initializationDelay);
    }
}