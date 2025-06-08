using UnityEngine;
using System.IO;

/// <summary>
/// Manages persistent game data and player preferences.
/// Handles save/load operations for game progress and calibration data.
/// </summary>
public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    private const string SAVE_FILE_NAME = "dungeonyou_save.json";
    private const string CALIBRATION_FILE_NAME = "dungeonyou_calibration.json";
    private string savePath;
    private string calibrationPath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialize save paths
        savePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        calibrationPath = Path.Combine(Application.persistentDataPath, CALIBRATION_FILE_NAME);
        
        Debug.Log($"[SaveSystem] Save path: {savePath}");
    }

    #region Game Save/Load
    public void SaveGame(SaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log("[SaveSystem] Game saved successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to save game: {e.Message}");
        }
    }

    public SaveData LoadGame()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                Debug.Log($"[SaveSystem] Game loaded from {data.timestamp}");
                return data;
            }
            else
            {
                Debug.Log("[SaveSystem] No save file found");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to load game: {e.Message}");
            return null;
        }
    }

    public void DeleteSave()
    {
        try
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log("[SaveSystem] Save file deleted");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to delete save: {e.Message}");
        }
    }

    public bool HasSaveFile()
    {
        return File.Exists(savePath);
    }
    #endregion

    #region Calibration Save/Load
    public void SaveCalibrationData()
    {
        if (MRController.Instance == null) return;

        try
        {
            CalibrationData calibData = new CalibrationData
            {
                // Save MR anchor positions and room bounds
                anchorPosition = MRController.Instance.GetAnchorPosition(),
                anchorRotation = MRController.Instance.GetAnchorRotation(),
                roomBounds = MRController.Instance.GetRoomBounds(),
                isCalibrated = true
            };

            string json = JsonUtility.ToJson(calibData, true);
            File.WriteAllText(calibrationPath, json);
            Debug.Log("[SaveSystem] Calibration data saved");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to save calibration: {e.Message}");
        }
    }

    public CalibrationData LoadCalibrationData()
    {
        try
        {
            if (File.Exists(calibrationPath))
            {
                string json = File.ReadAllText(calibrationPath);
                CalibrationData data = JsonUtility.FromJson<CalibrationData>(json);
                Debug.Log("[SaveSystem] Calibration data loaded");
                return data;
            }
            else
            {
                Debug.Log("[SaveSystem] No calibration file found");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to load calibration: {e.Message}");
            return null;
        }
    }
    #endregion

    #region Player Preferences
    public void SavePreference(string key, object value)
    {
        if (value is int intValue)
            PlayerPrefs.SetInt(key, intValue);
        else if (value is float floatValue)
            PlayerPrefs.SetFloat(key, floatValue);
        else if (value is string stringValue)
            PlayerPrefs.SetString(key, stringValue);
        
        PlayerPrefs.Save();
    }

    public T LoadPreference<T>(string key, T defaultValue)
    {
        if (typeof(T) == typeof(int))
            return (T)(object)PlayerPrefs.GetInt(key, (int)(object)defaultValue);
        else if (typeof(T) == typeof(float))
            return (T)(object)PlayerPrefs.GetFloat(key, (float)(object)defaultValue);
        else if (typeof(T) == typeof(string))
            return (T)(object)PlayerPrefs.GetString(key, (string)(object)defaultValue);
        
        return defaultValue;
    }
    #endregion
}

// Calibration data structure
[System.Serializable]
public class CalibrationData
{
    public Vector3 anchorPosition;
    public Quaternion anchorRotation;
    public Bounds roomBounds;
    public bool isCalibrated;
} 