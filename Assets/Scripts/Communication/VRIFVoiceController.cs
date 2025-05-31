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
    }

    public bool HasCommand() => commandQueue.Count > 0;
    public string GetNextCommand() => commandQueue.Count > 0 ? commandQueue.Dequeue() : null;
} 