using Pathfinding;
using UnityEngine;

public class BoarController : MonoBehaviour
{
    public AIPath aiPath;
    [SerializeField] private SpriteRenderer spriteRenderer;

    #region Animation
    [SerializeField] private Animator animator;
    private int currentAnimation;

    private int boarIdle = Animator.StringToHash("BoarIdle");
    private int boarRun = Animator.StringToHash("BoarRun");
    #endregion

    private void Start()
    {
        if (aiPath == null) aiPath = GetComponent<AIPath>();
    }

    private void Update()
    {
        UpdateDirection();
        UpdateAnimation(currentAnimation);
    }

    private void UpdateDirection()
    {
        Vector2 velocity = aiPath.desiredVelocity;

        if (velocity.sqrMagnitude < 0.01f)
        {
            currentAnimation = boarIdle;
            return;
        }

        currentAnimation = boarRun;
        spriteRenderer.flipX = velocity.x > 0;
    }

    private void UpdateAnimation(int animation)
    {
        animator.CrossFade(animation, 0);
    }
}
