using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Handles cloud-based LLM communication and data synchronization.
/// Manages online/offline modes and backup communication.
/// </summary>
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    public bool IsOnline { get; private set; } = true;

    public delegate void NetworkStateChanged(bool online);
    public static event NetworkStateChanged OnNetworkStateChanged;

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

    public void SetOnline(bool online)
    {
        if (IsOnline == online) return;
        IsOnline = online;
        OnNetworkStateChanged?.Invoke(online);
    }

    public void SendLLMRequest(string prompt, System.Action<string> onResponse)
    {
        if (!IsOnline)
        {
            Debug.LogWarning("[NetworkManager] Offline mode: LLM request not sent.");
            onResponse?.Invoke(null);
            return;
        }
        StartCoroutine(SendLLMRequestCoroutine(prompt, onResponse));
    }

    private IEnumerator SendLLMRequestCoroutine(string prompt, System.Action<string> onResponse)
    {
        // Example: POST to a cloud LLM endpoint
        string url = "https://your-llm-endpoint.com/api/generate";
        WWWForm form = new WWWForm();
        form.AddField("prompt", prompt);
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("[NetworkManager] LLM request failed: " + www.error);
                onResponse?.Invoke(null);
            }
            else
            {
                onResponse?.Invoke(www.downloadHandler.text);
            }
        }
    }
} 