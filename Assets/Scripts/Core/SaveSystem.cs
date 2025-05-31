using UnityEngine;
using System;

/// <summary>
/// Persists game progress and hero state.
/// Handles saving/loading of dungeon progress, hero stats, and preferences.
/// </summary>
public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

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

    [Serializable]
    public class SaveData
    {
        public string dungeonSeed;
        public int heroLevel;
        public int heroHealth;
        public string[] inventory;
        public string preferencesJson;
    }

    private const string SaveKey = "VRDungeonSave";

    public void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
        Debug.Log("Game Saved");
    }

    public SaveData LoadGame()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return null;
        string json = PlayerPrefs.GetString(SaveKey);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log("Game Loaded");
        return data;
    }

    public void ClearSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        Debug.Log("Save Cleared");
    }
} 