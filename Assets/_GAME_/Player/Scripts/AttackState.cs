using UnityEngine;

public class AttackState : StateMachineBehaviour
{
    public string attackAreaName = "AttackArea";

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Lấy root từ Character (vì animator nằm trong Character)
        Transform root = animator.transform.root;

        // Tìm AttackArea trong root
        Transform attackArea = root.Find(attackAreaName);

        if (attackArea != null)
        {
            attackArea.gameObject.SetActive(false); // Tắt khi animation thoát
        }
        else
        {
            Debug.LogWarning($"AttackArea '{attackAreaName}' not found under root {root.name}");
        }

        
    }
}
