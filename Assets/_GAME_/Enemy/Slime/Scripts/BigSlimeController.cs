using Pathfinding;
using UnityEngine;

public class BigSlimeController : MonoBehaviour
{
    public AIPath aiPath;
    [SerializeField] private SpriteRenderer spriteRenderer;

    #region Animation
    [SerializeField] private Animator animator;
    private int animaitonChoice = Animator.StringToHash("BigSlime_WalkDown");

    private int slimeWalkDown = Animator.StringToHash("BigSlime_WalkDown");
    private int slimeWalkLeft = Animator.StringToHash("BigSlime_WalkLeft");
    private int slimeWalkUp = Animator.StringToHash("BigSlime_WalkUp");
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
            animaitonChoice = slimeWalkDown;
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
                animaitonChoice = slimeWalkDown;
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
