using UnityEngine;

public class WeaponListening : MonoBehaviour
{
    [SerializeField] private SpriteRenderer weaponSprite;

    public void swingWeapon()
    {
        weaponSprite.enabled = true;
    }

    public void disableWeapon()
    {
        weaponSprite.enabled = false;
    }
}
