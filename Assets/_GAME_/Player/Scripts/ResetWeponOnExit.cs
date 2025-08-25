using UnityEngine;

public class ResetWeaponOnExit : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerAnimationEvents events = animator.GetComponent<PlayerAnimationEvents>();
        if (events != null)
        {
            events.DisableAll(); // đảm bảo tắt weapon + effect khi thoát state
        }
    }
}
