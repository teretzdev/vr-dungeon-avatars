using UnityEngine;

/// <summary>
/// Adapts Edgar Pro generated dungeons to real-world MR space.
/// Scales dungeon dimensions and adjusts for VR comfort.
/// </summary>
public class DungeonScaler : MonoBehaviour
{
    [Header("Play Area Settings")]
    [SerializeField] private float targetPlayAreaWidth = 3.0f;
    [SerializeField] private float targetPlayAreaLength = 3.0f;
    [SerializeField] private float minScale = 0.1f;
    [SerializeField] private float maxScale = 2.0f;
    
    [Header("Hero Scaling")]
    [SerializeField] private float tinyHeroScale = 0.1f;
    [SerializeField] private bool autoScaleHero = true;
    
    [Header("MR Settings")]
    [SerializeField] private float mrPadding = 0.5f; // Buffer space from room boundaries
    [SerializeField] private bool centerInPlaySpace = true;

    public void ScaleDungeon(GameObject dungeonRoot)
    {
        if (dungeonRoot == null) return;
        
        // Calculate dungeon bounds
        Bounds bounds = CalculateBounds(dungeonRoot);
        
        // Calculate scale to fit play area
        float scaleX = targetPlayAreaWidth / bounds.size.x;
        float scaleZ = targetPlayAreaLength / bounds.size.z;
        float scale = Mathf.Min(scaleX, scaleZ);
        
        // Clamp scale to reasonable limits
        scale = Mathf.Clamp(scale, minScale, maxScale);
        
        // Apply scale
        dungeonRoot.transform.localScale = new Vector3(scale, scale, scale);
        
        Debug.Log($"[DungeonScaler] Dungeon scaled to fit play area: {scale}");
    }

    public void ScaleDungeonToMRSpace(GameObject dungeonRoot)
    {
        if (dungeonRoot == null || MRController.Instance == null) return;
        
        // Get room bounds from MR Controller
        Bounds roomBounds = MRController.Instance.GetRoomBounds();
        if (roomBounds.size == Vector3.zero)
        {
            Debug.LogWarning("[DungeonScaler] No room bounds available, using default scaling");
            ScaleDungeon(dungeonRoot);
            return;
        }
        
        // Calculate dungeon bounds
        Bounds dungeonBounds = CalculateBounds(dungeonRoot);
        
        // Account for padding
        float availableWidth = roomBounds.size.x - (mrPadding * 2);
        float availableLength = roomBounds.size.z - (mrPadding * 2);
        
        // Calculate scale to fit room with padding
        float scaleX = availableWidth / dungeonBounds.size.x;
        float scaleZ = availableLength / dungeonBounds.size.z;
        float scale = Mathf.Min(scaleX, scaleZ);
        
        // Clamp scale
        scale = Mathf.Clamp(scale, minScale, maxScale);
        
        // Apply scale
        dungeonRoot.transform.localScale = Vector3.one * scale;
        
        // Position dungeon at MR anchor
        Vector3 anchorPosition = MRController.Instance.GetAnchorPosition();
        dungeonRoot.transform.position = anchorPosition;
        
        // Center in play space if requested
        if (centerInPlaySpace)
        {
            CenterDungeonInBounds(dungeonRoot, roomBounds);
        }
        
        // Scale hero accordingly
        if (autoScaleHero)
        {
            MiniaturizeHero(scale);
        }
        
        Debug.Log($"[DungeonScaler] Dungeon scaled for MR space: {scale}, positioned at: {anchorPosition}");
    }

    public void MiniaturizeDungeon(GameObject dungeonRoot)
    {
        if (dungeonRoot == null) return;
        
        dungeonRoot.transform.localScale = Vector3.one * tinyHeroScale;
        Debug.Log("[DungeonScaler] Dungeon miniaturized for tiny hero perspective");
    }

    private void MiniaturizeHero(float dungeonScale)
    {
        if (MetaAvatarHero.Instance != null)
        {
            // Scale hero relative to dungeon
            float heroScale = tinyHeroScale * dungeonScale;
            MetaAvatarHero.Instance.SetAvatarScale(heroScale);
            
            Debug.Log($"[DungeonScaler] Hero scaled to: {heroScale}");
        }
    }

    private void CenterDungeonInBounds(GameObject dungeonRoot, Bounds roomBounds)
    {
        // Calculate current dungeon bounds after scaling
        Bounds scaledBounds = CalculateBounds(dungeonRoot);
        
        // Calculate offset to center
        Vector3 dungeonCenter = scaledBounds.center;
        Vector3 roomCenter = roomBounds.center;
        Vector3 offset = roomCenter - dungeonCenter;
        offset.y = 0; // Keep at floor level
        
        // Apply offset
        dungeonRoot.transform.position += offset;
    }

    private Bounds CalculateBounds(GameObject root)
    {
        var renderers = root.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) 
            return new Bounds(root.transform.position, Vector3.one);
            
        Bounds bounds = renderers[0].bounds;
        foreach (var renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        
        return bounds;
    }
    
    public float GetCurrentScale()
    {
        return transform.localScale.x;
    }
} 