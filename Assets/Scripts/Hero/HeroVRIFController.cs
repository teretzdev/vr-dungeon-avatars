using UnityEngine;
using BNG;
using System.Collections.Generic;

/// <summary>
/// Bridges hero actions with VRIF weapon/interaction systems.
/// Manages inventory and combat animations.
/// </summary>
public class HeroVRIFController : MonoBehaviour
{
    public static HeroVRIFController Instance { get; private set; }

    [Header("Hero References")]
    private Transform heroTransform;
    private GameObject heroGameObject;
    
    [Header("VRIF Components")]
    [SerializeField] private Grabber leftHandGrabber;
    [SerializeField] private Grabber rightHandGrabber;
    [SerializeField] private BNGPlayerController playerController;
    
    [Header("Inventory")]
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private int maxInventorySize = 10;
    private List<Grabbable> inventory = new List<Grabbable>();
    
    [Header("Combat")]
    [SerializeField] private Transform weaponHolster;
    private Grabbable currentWeapon;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetHeroTransform(Transform hero)
    {
        heroTransform = hero;
        heroGameObject = hero.gameObject;
        
        // Set up VRIF grabbers on the hero
        SetupHeroGrabbers();
        
        Debug.Log("[HeroVRIFController] Hero transform set and VRIF configured");
    }

    private void SetupHeroGrabbers()
    {
        if (heroTransform == null) return;
        
        // Find or create hand transforms
        Transform leftHand = heroTransform.Find("LeftHand");
        Transform rightHand = heroTransform.Find("RightHand");
        
        if (leftHand == null)
        {
            GameObject leftHandObj = new GameObject("LeftHand");
            leftHandObj.transform.SetParent(heroTransform);
            leftHandObj.transform.localPosition = new Vector3(-0.2f, 0.8f, 0.3f);
            leftHand = leftHandObj.transform;
        }
        
        if (rightHand == null)
        {
            GameObject rightHandObj = new GameObject("RightHand");
            rightHandObj.transform.SetParent(heroTransform);
            rightHandObj.transform.localPosition = new Vector3(0.2f, 0.8f, 0.3f);
            rightHand = rightHandObj.transform;
        }
        
        // Add grabbers if not present
        if (leftHandGrabber == null)
        {
            leftHandGrabber = leftHand.gameObject.AddComponent<Grabber>();
            ConfigureGrabber(leftHandGrabber, GrabButton.Trigger);
        }
        
        if (rightHandGrabber == null)
        {
            rightHandGrabber = rightHand.gameObject.AddComponent<Grabber>();
            ConfigureGrabber(rightHandGrabber, GrabButton.Trigger);
        }
    }

    private void ConfigureGrabber(Grabber grabber, GrabButton grabButton)
    {
        grabber.GrabButton = grabButton;
        grabber.GrabRadius = 0.3f;
        grabber.GrabLayer = LayerMask.GetMask("Grabbable", "Default");
        grabber.RemoteGrabbing = true;
        grabber.RemoteGrabDistance = 5f;
    }

    public void PickupWeapon(GameObject weapon)
    {
        if (weapon == null) return;
        
        Grabbable grabbable = weapon.GetComponent<Grabbable>();
        if (grabbable == null)
        {
            Debug.LogWarning($"[HeroVRIFController] {weapon.name} is not grabbable!");
            return;
        }
        
        // Drop current weapon if holding one
        if (currentWeapon != null)
        {
            DropWeapon();
        }
        
        // Grab with right hand
        if (rightHandGrabber != null)
        {
            rightHandGrabber.GrabGrabbable(grabbable);
            currentWeapon = grabbable;
            
            // Trigger combat animation
            if (MetaAvatarAnimator.Instance != null)
            {
                MetaAvatarAnimator.Instance.PlayCombatAnimation("equipWeapon");
            }
        }
        
        Debug.Log($"[HeroVRIFController] Hero picked up weapon: {weapon.name}");
    }

    public void DropWeapon()
    {
        if (currentWeapon != null && rightHandGrabber != null)
        {
            rightHandGrabber.TryRelease();
            currentWeapon = null;
        }
    }

    public void UseItem(string itemName)
    {
        // Find item in inventory
        Grabbable item = inventory.Find(g => g.name.Contains(itemName));
        if (item != null)
        {
            // Use item based on type
            if (item.GetComponent<HealthPotion>() != null)
            {
                item.GetComponent<HealthPotion>().Use();
                inventory.Remove(item);
                Destroy(item.gameObject);
            }
            
            Debug.Log($"[HeroVRIFController] Hero used item: {itemName}");
        }
        else
        {
            Debug.Log($"[HeroVRIFController] Item not found in inventory: {itemName}");
        }
    }

    public void AddToInventory(Grabbable item)
    {
        if (inventory.Count < maxInventorySize)
        {
            inventory.Add(item);
            
            // Move item to inventory container
            if (inventoryContainer != null)
            {
                item.transform.SetParent(inventoryContainer);
                item.gameObject.SetActive(false);
            }
            
            // Update UI
            UpdateInventoryUI();
        }
        else
        {
            Debug.Log("[HeroVRIFController] Inventory full!");
        }
    }

    private void UpdateInventoryUI()
    {
        if (VRIFUIManager.Instance != null)
        {
            string[] itemNames = new string[inventory.Count];
            for (int i = 0; i < inventory.Count; i++)
            {
                itemNames[i] = inventory[i].name;
            }
            VRIFUIManager.Instance.UpdateInventory(itemNames);
        }
    }

    public void PlayCombatAnimation(string animName)
    {
        // Trigger animation on Meta Avatar
        if (MetaAvatarAnimator.Instance != null)
        {
            MetaAvatarAnimator.Instance.PlayCombatAnimation(animName);
        }
        
        // Handle weapon-specific animations
        if (currentWeapon != null)
        {
            var weaponComponent = currentWeapon.GetComponent<RaycastWeapon>();
            if (weaponComponent != null && animName == "attack")
            {
                weaponComponent.Shoot();
            }
        }
        
        Debug.Log($"[HeroVRIFController] Playing combat animation: {animName}");
    }

    public bool HasWeapon()
    {
        return currentWeapon != null;
    }

    public Grabbable GetCurrentWeapon()
    {
        return currentWeapon;
    }
}

// Placeholder health potion component
public class HealthPotion : MonoBehaviour
{
    public int healAmount = 25;
    
    public void Use()
    {
        if (EmeraldHeroAI.Instance != null)
        {
            // Heal the hero
            Debug.Log($"[HealthPotion] Healing hero for {healAmount} health");
        }
    }
} 