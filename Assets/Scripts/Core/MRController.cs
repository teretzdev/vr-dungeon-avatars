using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Management;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

/// <summary>
/// Manages mixed reality features and passthrough integration.
/// Handles spatial anchoring, calibration, and MR/VR mode switching.
/// </summary>
public class MRController : MonoBehaviour
{
    public static MRController Instance { get; private set; }

    public enum MRMode { VR, MR }
    public MRMode CurrentMode { get; private set; } = MRMode.VR;

    [Header("References")]
    public ARSession arSession;
    public ARAnchorManager anchorManager;
    public XROrigin xrOrigin;

    [Header("Calibration")]
    private ARAnchor currentAnchor;
    private Bounds currentRoomBounds = new Bounds(Vector3.zero, Vector3.one * 5f);

    public delegate void MRModeChanged(MRMode newMode);
    public static event MRModeChanged OnMRModeChanged;

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
        // Default to VR mode
        SetMode(MRMode.VR);
        
        // Try to load calibration data
        LoadCalibration();
    }

    public void SetMode(MRMode mode)
    {
        if (CurrentMode == mode) return;
        CurrentMode = mode;
        if (mode == MRMode.MR)
        {
            EnableMR();
        }
        else
        {
            EnableVR();
        }
        OnMRModeChanged?.Invoke(mode);
    }

    private void EnableMR()
    {
        if (arSession != null) arSession.enabled = true;
        if (anchorManager != null) anchorManager.enabled = true;
        // Enable passthrough if available (Quest 3 specific)
        // XR Management/AR Foundation handles camera background
    }

    private void EnableVR()
    {
        if (arSession != null) arSession.enabled = false;
        if (anchorManager != null) anchorManager.enabled = false;
        // Disable passthrough
    }

    public void CalibrateMR(Vector3 realWorldPosition)
    {
        // Place anchor at real world position for dungeon placement
        if (anchorManager != null)
        {
            // Remove existing anchor if any
            if (currentAnchor != null)
            {
                Destroy(currentAnchor.gameObject);
            }

            // Create a new GameObject at the desired position
            GameObject anchorObject = new GameObject("MR Anchor");
            anchorObject.transform.position = realWorldPosition;
            anchorObject.transform.rotation = Quaternion.identity;

            // Add ARAnchor component
            currentAnchor = anchorObject.AddComponent<ARAnchor>();

            if (currentAnchor != null)
            {
                // Notify dungeon system to use this anchor
                Debug.Log("MR Calibrated: Anchor placed at " + realWorldPosition);
                
                // Save calibration
                SaveSystem.Instance?.SaveCalibrationData();
            }
        }
    }

    public void SetRoomBounds(Bounds bounds)
    {
        currentRoomBounds = bounds;
        Debug.Log($"[MRController] Room bounds set: {bounds.size}");
    }

    #region Calibration Data Methods
    public Vector3 GetAnchorPosition()
    {
        return currentAnchor != null ? currentAnchor.transform.position : Vector3.zero;
    }

    public Quaternion GetAnchorRotation()
    {
        return currentAnchor != null ? currentAnchor.transform.rotation : Quaternion.identity;
    }

    public Bounds GetRoomBounds()
    {
        return currentRoomBounds;
    }

    public bool IsCalibrated()
    {
        return currentAnchor != null;
    }

    private void LoadCalibration()
    {
        if (SaveSystem.Instance == null) return;
        
        var calibData = SaveSystem.Instance.LoadCalibrationData();
        if (calibData != null && calibData.isCalibrated)
        {
            // Restore calibration
            CalibrateMR(calibData.anchorPosition);
            if (currentAnchor != null)
            {
                currentAnchor.transform.rotation = calibData.anchorRotation;
            }
            currentRoomBounds = calibData.roomBounds;
            
            Debug.Log("[MRController] Calibration restored from save");
        }
    }
    #endregion
} 