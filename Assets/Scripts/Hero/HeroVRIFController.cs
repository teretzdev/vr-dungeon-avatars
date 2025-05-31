using UnityEngine;

/// <summary>
/// Bridges hero actions with VRIF weapon/interaction systems.
/// Manages inventory and combat animations.
/// </summary>
public class HeroVRIFController : MonoBehaviour
{
    public static HeroVRIFController Instance { get; private set; }

    // Reference to VRIF weapon system (replace with actual VRIF type)
    public MonoBehaviour vrifWeaponSystem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PickupWeapon(GameObject weapon)
    {
        // Example: VRIF pickup
        // vrifWeaponSystem.Pickup(weapon);
        Debug.Log($"Hero picked up weapon: {weapon.name}");
    }

    public void UseItem(string itemName)
    {
        // Example: VRIF item use
        Debug.Log($"Hero used item: {itemName}");
    }

    public void PlayCombatAnimation(string animName)
    {
        // Example: Trigger animation on Meta Avatar
        Debug.Log($"Playing combat animation: {animName}");
    }
} 