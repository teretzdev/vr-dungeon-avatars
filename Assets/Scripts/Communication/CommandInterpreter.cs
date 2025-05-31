using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Translates voice commands into Emerald AI behaviors using a tiny LLM.
/// Maintains command history and context.
/// </summary>
public class CommandInterpreter : MonoBehaviour
{
    public static CommandInterpreter Instance { get; private set; }

    private List<string> commandHistory = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void InterpretCommand(string command)
    {
        commandHistory.Add(command);
        string lower = command.ToLower();
        if (lower.Contains("move"))
        {
            // Example: Move forward
            EmeraldHeroAI.Instance.MoveTo(Vector3.forward * 2f);
        }
        else if (lower.Contains("attack"))
        {
            // Example: Attack nearest enemy
            // EmeraldHeroAI.Instance.AttackTarget(FindNearestEnemy());
        }
        else if (lower.Contains("pickup") || lower.Contains("grab"))
        {
            // Example: Pickup item
            // HeroVRIFController.Instance.PickupWeapon(FindNearestWeapon());
        }
        else
        {
            // Fallback to LLM/cloud if available
            NetworkManager.Instance.SendLLMRequest(command, OnLLMResponse);
        }
        Debug.Log($"Command interpreted: {command}");
    }

    private void OnLLMResponse(string response)
    {
        Debug.Log($"LLM Response: {response}");
        // Optionally, parse and trigger further actions
    }
} 