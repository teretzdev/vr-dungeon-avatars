using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Integrates voice commands with VRIF input system.
/// Handles speech-to-text and command queuing.
/// </summary>
public class VRIFVoiceController : MonoBehaviour
{
    public static VRIFVoiceController Instance { get; private set; }

    private Queue<string> commandQueue = new Queue<string>();
    public bool isListening = false;
    
    // Dictionary to store registered voice commands and their callbacks
    private Dictionary<string, System.Action> registeredCommands = new Dictionary<string, System.Action>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        // Example: Start/stop listening with a button (replace with VRIF input)
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (!isListening) StartListening();
            else StopListening();
        }
    }

    public void StartListening()
    {
        isListening = true;
        Debug.Log("Voice listening started.");
        // TODO: Start Quest 3/Unity voice recognition
    }

    public void StopListening()
    {
        isListening = false;
        Debug.Log("Voice listening stopped.");
        // TODO: Stop voice recognition
    }

    // This would be called by the voice recognition system
    public void OnVoiceRecognized(string text)
    {
        commandQueue.Enqueue(text);
        Debug.Log($"Voice command recognized: {text}");
        
        // Check if this is a registered command and execute it
        ProcessRegisteredCommand(text);
    }
    
    public void RegisterCommand(string command, System.Action callback)
    {
        if (!string.IsNullOrEmpty(command) && callback != null)
        {
            // Convert to lowercase for case-insensitive matching
            string normalizedCommand = command.ToLower();
            
            if (registeredCommands.ContainsKey(normalizedCommand))
            {
                Debug.LogWarning($"Command '{command}' is already registered. Overwriting.");
            }
            
            registeredCommands[normalizedCommand] = callback;
            Debug.Log($"Voice command registered: {command}");
        }
    }
    
    public void UnregisterCommand(string command)
    {
        if (!string.IsNullOrEmpty(command))
        {
            string normalizedCommand = command.ToLower();
            if (registeredCommands.Remove(normalizedCommand))
            {
                Debug.Log($"Voice command unregistered: {command}");
            }
        }
    }
    
    private void ProcessRegisteredCommand(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        
        string normalizedText = text.ToLower();
        
        // Check for exact match
        if (registeredCommands.TryGetValue(normalizedText, out System.Action callback))
        {
            callback?.Invoke();
            return;
        }
        
        // Check if the text contains any registered command
        foreach (var kvp in registeredCommands)
        {
            if (normalizedText.Contains(kvp.Key))
            {
                kvp.Value?.Invoke();
                return;
            }
        }
    }

    public bool HasCommand() => commandQueue.Count > 0;
    public string GetNextCommand() => commandQueue.Count > 0 ? commandQueue.Dequeue() : null;
} 