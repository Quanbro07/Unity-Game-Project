using UnityEngine;
using Pathfinding;

public class EnemyGFX : MonoBehaviour
{
    public AIPath aiPath;
    [SerializeField] private SpriteRenderer spriteRenderer;

    #region Animation
    [SerializeField] private Animator animator;
    private int animaitonChoice = Animator.StringToHash("Slime_Sleep");

    private int slimeIdle = Animator.StringToHash("Slime_Idle");
    private int slimeWalkLeft = Animator.StringToHash("Slime_Walk_Left");
    private int slimeWalkUp = Animator.StringToHash("Slime_Walk_Up");
    #endregion

    private void Update()
    {
        UpdateDirection();
        UpdateAnimation(animaitonChoice);
    }

    private void UpdateDirection()
    {
        Vector2 velocity = aiPath.desiredVelocity;

        if (velocity.sqrMagnitude < 0.01f)
        {
            animaitonChoice = slimeIdle;
            return;
        }

        if (Mathf.Abs(velocity.y) > Mathf.Abs(velocity.x))
        {
            // Ưu tiên hướng dọc (lên/xuống)
            if (velocity.y > 0)
            {
                animaitonChoice = slimeWalkUp;
            }
            else
            {
                animaitonChoice = slimeIdle;
            }
        }
        else
        {
            // Hướng ngang: trái/phải đều dùng WalkLeft + flipX
            animaitonChoice = slimeWalkLeft;
            spriteRenderer.flipX = velocity.x > 0;
        }
    }

    private void UpdateAnimation(int animation)
    {
        animator.CrossFade(animation, 0);
    }
}
