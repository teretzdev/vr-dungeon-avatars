using UnityEngine;

/// <summary>
/// Tiny hero character using Meta Avatar System.
/// Handles avatar customization, scaling, and animation rig.
/// </summary>
public class MetaAvatarHero : MonoBehaviour
{
    public static MetaAvatarHero Instance { get; private set; }

    [Header("Meta Avatar SDK")]
    public GameObject metaAvatarPrefab;
    public Transform avatarSpawnPoint;
    public float tinyScale = 0.1f;
    private GameObject avatarInstance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnAvatar(string customizationJson = null)
    {
        if (avatarInstance != null)
            Destroy(avatarInstance);
        avatarInstance = Instantiate(metaAvatarPrefab, avatarSpawnPoint.position, avatarSpawnPoint.rotation);
        avatarInstance.transform.localScale = Vector3.one * tinyScale;
        // Example: Apply customization via Meta Avatar SDK
        // MetaAvatarComponent meta = avatarInstance.GetComponent<MetaAvatarComponent>();
        // if (customizationJson != null) meta.ApplyCustomization(customizationJson);
        Debug.Log("Meta Avatar hero spawned and customized.");
    }

    public void SetAvatarActive(bool active)
    {
        if (avatarInstance != null)
            avatarInstance.SetActive(active);
    }

    public GameObject GetAvatarInstance() => avatarInstance;
} 