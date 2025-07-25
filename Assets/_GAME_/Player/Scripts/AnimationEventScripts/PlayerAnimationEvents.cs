using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private WeaponListening weaponListening;
    [SerializeField] private WeaponEffectListening weaponEffectListening;
    [SerializeField] private PlayerAttack playerAttack; 

    public void EnableAll()
    {
        if (weaponListening != null)
        {
            weaponListening.swingWeapon(playerAttack.currentCombo);
        }

        if (weaponEffectListening != null)
        {
            weaponEffectListening.EnableEffect(playerAttack.currentCombo);
        }
    }

    public void DisableAll()
    {   
        Debug.Log("DisableAll");
        if (weaponListening != null)
        {
            weaponListening.disableWeapon(playerAttack.currentCombo);
        }

        if (weaponEffectListening != null)
        {
            weaponEffectListening.DisableEffect(playerAttack.currentCombo);
        }
    }

}
