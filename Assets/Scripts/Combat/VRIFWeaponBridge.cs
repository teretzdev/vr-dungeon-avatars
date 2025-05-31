using UnityEngine;

/// <summary>
/// Bridges VRIF weapon system with tiny hero scale.
/// Handles weapon scaling, pickup/drop, and effects.
/// </summary>
public class VRIFWeaponBridge : MonoBehaviour
{
    public float tinyWeaponScale = 0.1f;

    public void AdaptWeapon(GameObject weapon)
    {
        weapon.transform.localScale = Vector3.one * tinyWeaponScale;
        Debug.Log($"Weapon adapted for tiny hero: {weapon.name}");
    }

    public void PickupWeapon(GameObject weapon, GameObject hero)
    {
        AdaptWeapon(weapon);
        weapon.transform.SetParent(hero.transform);
        weapon.transform.localPosition = Vector3.zero;
        Debug.Log($"Weapon picked up by hero: {weapon.name}");
    }

    public void DropWeapon(GameObject weapon)
    {
        weapon.transform.SetParent(null);
        Debug.Log($"Weapon dropped: {weapon.name}");
    }

    public void PlayWeaponEffect(string effectName)
    {
        // Example: Play weapon effect (VFX/SFX)
        Debug.Log($"Weapon effect played: {effectName}");
    }
} 