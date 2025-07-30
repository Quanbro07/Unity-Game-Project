using UnityEngine;

public class WeaponEffectListening : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] weaponEffect;


    public void EnableEffect(int currentCombo)
    {
        weaponEffect[currentCombo-1].enabled = true;
    }

    public void DisableEffect(int currentCombo)
    {
        weaponEffect[currentCombo - 1].enabled = false;
    }
}
