using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Management;
using UnityEngine.XR.Interaction.Toolkit;

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
            ARAnchor anchor = anchorManager.AddAnchor(new Pose(realWorldPosition, Quaternion.identity));
            if (anchor != null)
            {
                // Notify dungeon system to use this anchor
                Debug.Log("MR Calibrated: Anchor placed at " + realWorldPosition);
            }
        }
    }
} 