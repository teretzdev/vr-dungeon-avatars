using UnityEngine;

/// <summary>
/// Adapts Edgar Pro generated dungeons to real-world MR space.
/// Scales dungeon dimensions and adjusts for VR comfort.
/// </summary>
public class DungeonScaler : MonoBehaviour
{
    public float targetPlayAreaWidth = 3.0f;
    public float targetPlayAreaLength = 3.0f;
    public float tinyHeroScale = 0.1f;

    public void ScaleDungeon(GameObject dungeonRoot)
    {
        // Example: Scale dungeon to fit play area
        Bounds bounds = CalculateBounds(dungeonRoot);
        float scaleX = targetPlayAreaWidth / bounds.size.x;
        float scaleZ = targetPlayAreaLength / bounds.size.z;
        float scale = Mathf.Min(scaleX, scaleZ);
        dungeonRoot.transform.localScale = new Vector3(scale, scale, scale);
        Debug.Log($"Dungeon scaled to fit play area: {scale}");
    }

    public void MiniaturizeDungeon(GameObject dungeonRoot)
    {
        dungeonRoot.transform.localScale = Vector3.one * tinyHeroScale;
        Debug.Log("Dungeon miniaturized for tiny hero perspective");
    }

    private Bounds CalculateBounds(GameObject root)
    {
        var renderers = root.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(root.transform.position, Vector3.one);
        Bounds bounds = renderers[0].bounds;
        foreach (var r in renderers)
            bounds.Encapsulate(r.bounds);
        return bounds;
    }
} 