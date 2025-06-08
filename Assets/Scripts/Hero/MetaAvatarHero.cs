using UnityEngine;
using Oculus.Avatar2;

/// <summary>
/// Tiny hero character using Meta Avatar System.
/// Handles avatar customization, scaling, and animation rig.
/// </summary>
public class MetaAvatarHero : MonoBehaviour
{
    public static MetaAvatarHero Instance { get; private set; }

    [Header("Meta Avatar SDK")]
    [SerializeField] private GameObject metaAvatarPrefab;
    [SerializeField] private Transform avatarSpawnPoint;
    [SerializeField] private float tinyScale = 0.1f;
    
    [Header("Avatar Settings")]
    [SerializeField] private OvrAvatarEntity avatarEntity;
    [SerializeField] private bool useDefaultAvatar = true;
    [SerializeField] private ulong oculusUserId = 0;
    
    private GameObject avatarInstance;
    private bool isSpawned = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnHero()
    {
        if (!isSpawned)
        {
            SpawnAvatar();
        }
    }

    public void SpawnAvatar(string customizationJson = null)
    {
        if (avatarInstance != null)
            Destroy(avatarInstance);
            
        // Determine spawn position
        Vector3 spawnPos = avatarSpawnPoint != null ? avatarSpawnPoint.position : transform.position;
        Quaternion spawnRot = avatarSpawnPoint != null ? avatarSpawnPoint.rotation : transform.rotation;
        
        // Spawn avatar
        avatarInstance = Instantiate(metaAvatarPrefab, spawnPos, spawnRot);
        avatarInstance.transform.localScale = Vector3.one * tinyScale;
        avatarInstance.name = "Tiny Hero Avatar";
        
        // Set up Meta Avatar Entity
        avatarEntity = avatarInstance.GetComponent<OvrAvatarEntity>();
        if (avatarEntity != null)
        {
            if (useDefaultAvatar)
            {
                // Use default avatar
                avatarEntity.CreateEntity(new CAPI.ovrAvatar2EntityCreateInfo
                {
                    features = CAPI.ovrAvatar2EntityFeatures.UseDefaultAvatar
                });
            }
            else if (oculusUserId != 0)
            {
                // Load user's avatar
                avatarEntity._userId = oculusUserId;
                avatarEntity.LoadUser();
            }
        }
        
        // Connect to other hero systems
        ConnectHeroSystems();
        
        isSpawned = true;
        Debug.Log("[MetaAvatarHero] Hero avatar spawned and configured");
    }

    private void ConnectHeroSystems()
    {
        // Connect animator
        if (MetaAvatarAnimator.Instance != null)
        {
            MetaAvatarAnimator.Instance.SetAvatar(avatarInstance);
        }
        
        // Connect VRIF controller
        if (HeroVRIFController.Instance != null)
        {
            HeroVRIFController.Instance.SetHeroTransform(avatarInstance.transform);
        }
        
        // Connect Emerald AI
        if (EmeraldHeroAI.Instance != null)
        {
            EmeraldHeroAI.Instance.SetHeroGameObject(avatarInstance);
        }
    }

    public void SetAvatarActive(bool active)
    {
        if (avatarInstance != null)
            avatarInstance.SetActive(active);
    }

    public GameObject GetAvatarInstance() => avatarInstance;
    
    public Transform GetAvatarTransform()
    {
        return avatarInstance != null ? avatarInstance.transform : transform;
    }
    
    public void SetAvatarPosition(Vector3 position)
    {
        if (avatarInstance != null)
        {
            avatarInstance.transform.position = position;
        }
    }
    
    public void SetAvatarScale(float scale)
    {
        tinyScale = scale;
        if (avatarInstance != null)
        {
            avatarInstance.transform.localScale = Vector3.one * tinyScale;
        }
    }
} 