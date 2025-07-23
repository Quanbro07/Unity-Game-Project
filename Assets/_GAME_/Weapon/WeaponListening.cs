using UnityEngine;

public class WeaponListening : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] weaponSprite;

    
    public void swingWeapon(int currentCombo)
    {
        weaponSprite[currentCombo-1].enabled = true;
    }

    public void disableWeapon(int currentCombo)
    {
        weaponSprite[currentCombo-1].enabled = false;
    }
}
