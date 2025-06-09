using UnityEngine;

public class WeaponEffectListening : MonoBehaviour
{
    [SerializeField] private SpriteRenderer weaponEffect;

    public void EnableEffect()
    {
        weaponEffect.enabled = true;
    }

    public void DisableEffect()
    {
        weaponEffect.enabled = false;
    }
}
