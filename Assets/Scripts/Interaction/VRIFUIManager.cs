using UnityEngine;
using TMPro;

/// <summary>
/// Manages UI using VRIF's VR interface system.
/// Displays hero health, inventory, and command feedback.
/// </summary>
public class VRIFUIManager : MonoBehaviour
{
    public static VRIFUIManager Instance { get; private set; }

    public Canvas worldSpaceCanvas;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI inventoryText;
    public TextMeshProUGUI commandFeedbackText;
    public TextMeshProUGUI goldText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void UpdateHealth(int health)
    {
        if (healthText != null)
            healthText.text = $"Health: {health}";
    }

    public void UpdateInventory(string[] items)
    {
        if (inventoryText != null)
            inventoryText.text = "Inventory:\n" + string.Join(", ", items);
    }

    public void UpdateGoldDisplay(int gold)
    {
        if (goldText != null)
            goldText.text = $"Gold: {gold}";
    }

    public void ShowCommandFeedback(string feedback)
    {
        if (commandFeedbackText != null)
            commandFeedbackText.text = feedback;
    }

    public void SetUIPosition(Vector3 position)
    {
        if (worldSpaceCanvas != null)
            worldSpaceCanvas.transform.position = position;
    }
} 