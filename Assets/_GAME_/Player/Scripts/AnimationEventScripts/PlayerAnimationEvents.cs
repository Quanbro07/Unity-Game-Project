using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private WeaponListening weaponListening;
    [SerializeField] private WeaponEffectListening weaponEffectListening;
    private void EnableAll()
    {
        if (weaponListening != null)
        {
            weaponListening.swingWeapon();
        }

        if (weaponEffectListening != null)
        {
            weaponEffectListening.EnableEffect();
        }
    }

    private void DisableAll()
    {   
        Debug.Log("DisableAll");
        if (weaponListening != null)
        {
            weaponListening.disableWeapon();
        }

        if (weaponEffectListening != null)
        {
            weaponEffectListening.DisableEffect();
        }
    }

}
