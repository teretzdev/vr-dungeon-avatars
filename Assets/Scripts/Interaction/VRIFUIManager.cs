using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Manages UI using VRIF's VR interface system.
/// Displays hero health, inventory, and command feedback.
/// </summary>
public class VRIFUIManager : MonoBehaviour
{
    public static VRIFUIManager Instance { get; private set; }

    [Header("Canvas")]
    public Canvas worldSpaceCanvas;
    public float uiFollowSpeed = 2f;
    public Vector3 uiOffset = new Vector3(0, 1.5f, 2f);
    
    [Header("Core UI Elements")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI inventoryText;
    public TextMeshProUGUI commandFeedbackText;
    
    [Header("Additional UI Elements")]
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI questText;
    public Image healthBar;
    public GameObject damageFlash;
    
    [Header("Settings")]
    public float feedbackDisplayDuration = 3f;
    public Color damageTextColor = Color.red;
    public Color healTextColor = Color.green;
    public Color normalTextColor = Color.white;
    
    private Transform playerTransform;
    private Coroutine feedbackCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        // Find player transform (assuming it has the "Player" tag)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        // Initialize UI
        UpdateHealth(100, 100);
        UpdateGold(0);
        UpdateInventory(new string[0]);
    }
    
    private void Update()
    {
        // Make UI follow player smoothly
        if (worldSpaceCanvas != null && playerTransform != null)
        {
            Vector3 targetPosition = playerTransform.position + 
                                   playerTransform.forward * uiOffset.z + 
                                   playerTransform.up * uiOffset.y +
                                   playerTransform.right * uiOffset.x;
                                   
            worldSpaceCanvas.transform.position = Vector3.Lerp(
                worldSpaceCanvas.transform.position, 
                targetPosition, 
                Time.deltaTime * uiFollowSpeed
            );
            
            // Make UI face the player
            worldSpaceCanvas.transform.LookAt(
                2 * worldSpaceCanvas.transform.position - playerTransform.position
            );
        }
    }

    #region Health Display
    public void UpdateHealth(int currentHealth, int maxHealth = 100)
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth}/{maxHealth}";
            
            // Update health bar if available
            if (healthBar != null)
            {
                healthBar.fillAmount = (float)currentHealth / maxHealth;
                
                // Change color based on health percentage
                float healthPercent = (float)currentHealth / maxHealth;
                if (healthPercent > 0.6f)
                    healthBar.color = Color.green;
                else if (healthPercent > 0.3f)
                    healthBar.color = Color.yellow;
                else
                    healthBar.color = Color.red;
            }
        }
    }
    
    public void ShowDamage(int damage)
    {
        ShowCommandFeedback($"-{damage} HP", damageTextColor);
        
        // Flash damage indicator
        if (damageFlash != null)
        {
            StartCoroutine(FlashDamage());
        }
    }
    
    private IEnumerator FlashDamage()
    {
        damageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        damageFlash.SetActive(false);
    }
    #endregion

    #region Currency Display
    public void UpdateGold(int gold)
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {gold}";
        }
    }
    
    public void ShowGoldChange(int amount)
    {
        if (amount > 0)
            ShowCommandFeedback($"+{amount} Gold", Color.yellow);
        else
            ShowCommandFeedback($"{amount} Gold", Color.red);
    }
    #endregion

    #region Inventory Display
    public void UpdateInventory(string[] items)
    {
        if (inventoryText != null)
        {
            if (items.Length == 0)
            {
                inventoryText.text = "Inventory: Empty";
            }
            else
            {
                // Format inventory with line breaks for readability
                string inventoryDisplay = "Inventory:\n";
                for (int i = 0; i < items.Length; i++)
                {
                    inventoryDisplay += $"• {items[i]}\n";
                    
                    // Limit displayed items to prevent UI overflow
                    if (i >= 5 && items.Length > 6)
                    {
                        inventoryDisplay += $"... and {items.Length - 6} more";
                        break;
                    }
                }
                inventoryText.text = inventoryDisplay;
            }
        }
    }
    #endregion

    #region Command Feedback
    public void ShowCommandFeedback(string feedback, Color? textColor = null)
    {
        if (commandFeedbackText != null)
        {
            commandFeedbackText.text = feedback;
            commandFeedbackText.color = textColor ?? normalTextColor;
            
            // Cancel previous fade if any
            if (feedbackCoroutine != null)
                StopCoroutine(feedbackCoroutine);
                
            // Start fade out after delay
            feedbackCoroutine = StartCoroutine(FadeFeedback());
        }
    }
    
    private IEnumerator FadeFeedback()
    {
        yield return new WaitForSeconds(feedbackDisplayDuration);
        
        // Fade out
        float fadeTime = 0.5f;
        float elapsed = 0;
        Color startColor = commandFeedbackText.color;
        
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsed / fadeTime);
            commandFeedbackText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        
        commandFeedbackText.text = "";
        commandFeedbackText.color = normalTextColor;
    }
    #endregion

    #region Room & Quest Display
    public void UpdateRoomName(string roomName)
    {
        if (roomNameText != null)
        {
            roomNameText.text = roomName;
            
            // Briefly highlight room name
            StartCoroutine(HighlightText(roomNameText, 2f));
        }
    }
    
    public void UpdateQuestText(string questInfo)
    {
        if (questText != null)
        {
            questText.text = questInfo;
        }
    }
    
    private IEnumerator HighlightText(TextMeshProUGUI text, float duration)
    {
        Color originalColor = text.color;
        text.color = Color.yellow;
        yield return new WaitForSeconds(duration);
        text.color = originalColor;
    }
    #endregion

    #region UI Positioning
    public void SetUIPosition(Vector3 position)
    {
        if (worldSpaceCanvas != null)
            worldSpaceCanvas.transform.position = position;
    }
    
    public void SetUIOffset(Vector3 offset)
    {
        uiOffset = offset;
    }
    
    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }
    #endregion

    #region Utility
    public void ShowNotification(string message, float duration = 2f)
    {
        ShowCommandFeedback(message);
    }
    
    public void ClearAllUI()
    {
        if (healthText != null) healthText.text = "";
        if (goldText != null) goldText.text = "";
        if (inventoryText != null) inventoryText.text = "";
        if (commandFeedbackText != null) commandFeedbackText.text = "";
        if (roomNameText != null) roomNameText.text = "";
        if (questText != null) questText.text = "";
    }
    #endregion
} 