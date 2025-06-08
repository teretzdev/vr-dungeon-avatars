using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manages UI using VRIF's VR interface system.
/// Displays hero health, inventory, and command feedback.
/// </summary>
public class VRIFUIManager : MonoBehaviour
{
    public static VRIFUIManager Instance { get; private set; }

    [Header("Canvas References")]
    [SerializeField] private Canvas worldSpaceCanvas;
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private Canvas calibrationCanvas;
    [SerializeField] private Canvas pauseMenuCanvas;
    
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI inventoryText;
    [SerializeField] private TextMeshProUGUI commandFeedbackText;
    [SerializeField] private Slider healthBar;
    
    [Header("Menu Elements")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject calibrationPanel;
    
    [Header("UI Settings")]
    [SerializeField] private float uiDistance = 2f;
    [SerializeField] private float feedbackDuration = 3f;
    
    private Coroutine feedbackCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Initialize UI states
        HideAllMenus();
    }

    #region Health & Status
    public void UpdateHealth(int health)
    {
        if (healthText != null)
            healthText.text = $"Health: {health}";
    }
    
    public void UpdateHealthDisplay(float currentHealth, float maxHealth)
    {
        if (healthText != null)
            healthText.text = $"Health: {currentHealth:F0}/{maxHealth:F0}";
            
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
    }

    public void UpdateInventory(string[] items)
    {
        if (inventoryText != null)
            inventoryText.text = "Inventory:\n" + string.Join(", ", items);
    }

    public void ShowCommandFeedback(string feedback)
    {
        if (commandFeedbackText != null)
        {
            commandFeedbackText.text = feedback;
            
            // Clear previous coroutine
            if (feedbackCoroutine != null)
                StopCoroutine(feedbackCoroutine);
                
            // Start fade out after duration
            feedbackCoroutine = StartCoroutine(FadeOutFeedback());
        }
    }
    
    private IEnumerator FadeOutFeedback()
    {
        yield return new WaitForSeconds(feedbackDuration);
        
        if (commandFeedbackText != null)
        {
            // Fade out animation
            float fadeTime = 0.5f;
            Color originalColor = commandFeedbackText.color;
            
            for (float t = 0; t < fadeTime; t += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
                commandFeedbackText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
            
            commandFeedbackText.text = "";
            commandFeedbackText.color = originalColor;
        }
    }
    #endregion

    #region Menu Management
    public void ShowMenu()
    {
        HideAllMenus();
        
        if (menuCanvas != null)
            menuCanvas.enabled = true;
            
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
            
        PositionMenuInFrontOfPlayer(menuCanvas);
    }
    
    public void HideMenu()
    {
        if (menuCanvas != null)
            menuCanvas.enabled = false;
            
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
    }
    
    public void ShowCalibrationUI()
    {
        HideAllMenus();
        
        if (calibrationCanvas != null)
            calibrationCanvas.enabled = true;
            
        if (calibrationPanel != null)
            calibrationPanel.SetActive(true);
            
        PositionMenuInFrontOfPlayer(calibrationCanvas);
    }
    
    public void HideCalibrationUI()
    {
        if (calibrationCanvas != null)
            calibrationCanvas.enabled = false;
            
        if (calibrationPanel != null)
            calibrationPanel.SetActive(false);
    }
    
    public void ShowPauseMenu()
    {
        HideAllMenus();
        
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.enabled = true;
            
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
            
        PositionMenuInFrontOfPlayer(pauseMenuCanvas);
    }
    
    public void HidePauseMenu()
    {
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.enabled = false;
            
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }
    
    private void HideAllMenus()
    {
        HideMenu();
        HideCalibrationUI();
        HidePauseMenu();
    }
    #endregion

    #region UI Positioning
    public void SetUIPosition(Vector3 position)
    {
        if (worldSpaceCanvas != null)
            worldSpaceCanvas.transform.position = position;
    }
    
    private void PositionMenuInFrontOfPlayer(Canvas canvas)
    {
        if (canvas == null) return;
        
        // Get player camera
        Camera playerCamera = Camera.main;
        if (playerCamera == null)
        {
            var vrPlayer = GameManager.Instance?.GetVRPlayer();
            if (vrPlayer != null)
            {
                playerCamera = vrPlayer.GetComponentInChildren<Camera>();
            }
        }
        
        if (playerCamera != null)
        {
            // Position canvas in front of player
            Vector3 forward = playerCamera.transform.forward;
            forward.y = 0;
            forward.Normalize();
            
            canvas.transform.position = playerCamera.transform.position + forward * uiDistance;
            canvas.transform.rotation = Quaternion.LookRotation(forward);
        }
    }
    
    private void Update()
    {
        // Keep world space HUD facing the player
        if (worldSpaceCanvas != null && worldSpaceCanvas.enabled)
        {
            Camera playerCamera = Camera.main;
            if (playerCamera != null)
            {
                Vector3 lookDirection = worldSpaceCanvas.transform.position - playerCamera.transform.position;
                lookDirection.y = 0;
                if (lookDirection != Vector3.zero)
                {
                    worldSpaceCanvas.transform.rotation = Quaternion.LookRotation(lookDirection);
                }
            }
        }
    }
    #endregion

    #region Button Callbacks
    public void OnStartGameClicked()
    {
        GameManager.Instance?.StartNewGame();
    }
    
    public void OnResumeClicked()
    {
        GameManager.Instance?.ResumeGame();
    }
    
    public void OnMainMenuClicked()
    {
        GameManager.Instance?.ReturnToMenu();
    }
    
    public void OnQuitClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void OnCalibrationComplete()
    {
        HideCalibrationUI();
        GameManager.Instance?.SwitchState(GameManager.GameState.Gameplay);
    }
    #endregion
} 