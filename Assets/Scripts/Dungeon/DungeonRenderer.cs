using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Handles visual representation of Edgar Pro dungeons in MR space.
/// Manages rendering, lighting, and LOD for performance.
/// </summary>
public class DungeonRenderer : MonoBehaviour
{
    public Light dungeonLight;
    public Material occlusionMaterial;
    public float lodDistance = 10f;

    private void Start()
    {
        SetupLighting();
    }

    public void ApplyOcclusion(GameObject dungeonRoot)
    {
        foreach (var r in dungeonRoot.GetComponentsInChildren<Renderer>())
        {
            r.material = occlusionMaterial;
        }
        Debug.Log("Dungeon occlusion material applied");
    }

    private void SetupLighting()
    {
        if (dungeonLight == null)
        {
            dungeonLight = new GameObject("DungeonLight").AddComponent<Light>();
            dungeonLight.type = LightType.Directional;
            dungeonLight.intensity = 0.8f;
        }
        dungeonLight.transform.rotation = Quaternion.Euler(50, -30, 0);
    }

    public void UpdateLOD(GameObject dungeonRoot, Transform player)
    {
        foreach (var r in dungeonRoot.GetComponentsInChildren<Renderer>())
        {
            float dist = Vector3.Distance(r.transform.position, player.position);
            r.enabled = dist < lodDistance;
        }
    }
} 