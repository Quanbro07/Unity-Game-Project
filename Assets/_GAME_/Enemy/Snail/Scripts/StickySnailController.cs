using Pathfinding;
using System.Collections;
using UnityEngine;

public class StickySnailController : MonoBehaviour
{
    public AIPath aiPath;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AIDestinationSetter destinationSetter;

    #region Animation
    [SerializeField] private Animator animator;
    private int snailIdle = Animator.StringToHash("SnailIdle");
    private int snailRun = Animator.StringToHash("SnailRun");
    private int currentAnimation;
    #endregion

    #region States
    public enum State { Wander, Flee, GoToFleePos }
    private State currentState = State.Wander;
    #endregion

    #region RandomWalk
    [SerializeField] private float wanderRadius = 3f;
    private Transform wanderTarget;
    private float wanderCooldown = 3f;
    private float wanderTimer;
    [SerializeField] private float idleDelay = 2f;
    private bool isWaitingToWander = false;
    #endregion

    #region Flee
    [SerializeField] private Transform player;
    [SerializeField] private float fleeDistance = 5f;
    private Transform fleeTarget;
    private bool isFleeing = false;
    #endregion

    private void Start()
    {
        // Tạo targets
        wanderTarget = new GameObject("SnailWanderTarget").transform;
        fleeTarget = new GameObject("SnailFleeTarget").transform;

        // Bắt đầu với random walk
        SetState(State.Wander);
    }

    private void Update()
    {
        if (currentState == State.Wander)
        {
            WanderLogic();
        }
        else if (currentState == State.Flee)
        {
            FleeLogic();
        }
        else if (currentState == State.GoToFleePos)
        {
            CheckReachedFleePos();
        }

        UpdateDirection();
        UpdateAnimation(currentAnimation);
    }

    private void UpdateDirection()
    {
        Vector2 velocity = aiPath.desiredVelocity;
        if (velocity.sqrMagnitude < 0.01f)
        {
            currentAnimation = snailIdle;
            return;
        }

        currentAnimation = snailRun;
        spriteRenderer.flipX = velocity.x > 0;
    }

    private void UpdateAnimation(int animation)
    {
        animator.CrossFade(animation, 0);
    }

    public void SetState(State newState)
    {
        currentState = newState;

        if (newState == State.Wander)
        {
            isFleeing = false;
            wanderTimer = 0;

            // Disable pathfinding ban đầu để có delay
            aiPath.enabled = false;
            destinationSetter.enabled = false;

            if (!isWaitingToWander)
            {
                StartCoroutine(DelayBeforeWander());
            }
        }
        else if (newState == State.Flee)
        {
            isFleeing = true;
            StopAllCoroutines();
            isWaitingToWander = false;

            aiPath.enabled = true;
            destinationSetter.enabled = true;
            destinationSetter.target = fleeTarget;
        }
        else if (newState == State.GoToFleePos)
        {
            // Tiếp tục đi tới vị trí flee cuối cùng
            isFleeing = false;
            StopAllCoroutines();
            isWaitingToWander = false;

            aiPath.enabled = true;
            destinationSetter.enabled = true;
            // Target vẫn là fleeTarget (vị trí cuối cùng)
        }
    }

    #region Wander Logic
    private void WanderLogic()
    {
        if (isWaitingToWander) return;

        if (!aiPath.enabled)
        {
            aiPath.enabled = true;
            destinationSetter.enabled = true;
        }

        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0 || aiPath.reachedEndOfPath || aiPath.remainingDistance < 0.2f)
        {
            SetRandomWanderTarget();
            wanderTimer = wanderCooldown;
        }
    }

    private void SetRandomWanderTarget()
    {
        Vector2 origin = transform.position;
        Vector2 randomPoint = origin + Random.insideUnitCircle * wanderRadius;
        wanderTarget.position = randomPoint;
        destinationSetter.target = wanderTarget;
    }

    private IEnumerator DelayBeforeWander()
    {
        isWaitingToWander = true;
        yield return new WaitForSeconds(idleDelay);

        if (currentState == State.Wander)
        {
            isWaitingToWander = false;
            SetRandomWanderTarget();
            aiPath.enabled = true;
            destinationSetter.enabled = true;
        }
    }
    #endregion

    #region Flee Logic
    private void FleeLogic()
    {
        if (player != null)
        {
            // Tính hướng chạy trốn (ngược lại với player)
            Vector3 dirAway = (transform.position - player.position).normalized;
            Vector3 targetPos = transform.position + dirAway * fleeDistance;
            fleeTarget.position = targetPos;
        }
    }

    public void StartFleeing()
    {
        SetState(State.Flee);
    }

    public void StopFleeing()
    {
        // Không chuyển ngay sang Wander, mà chuyển sang GoToFleePos
        SetState(State.GoToFleePos);
    }

    private void CheckReachedFleePos()
    {
        // Kiểm tra xem đã đến vị trí flee cuối cùng chưa
        if (aiPath.reachedEndOfPath || aiPath.remainingDistance < 0.5f)
        {
            // Đã đến vị trí flee, chuyển về Wander để bắt đầu delay
            SetState(State.Wander);
        }
    }
    #endregion
}