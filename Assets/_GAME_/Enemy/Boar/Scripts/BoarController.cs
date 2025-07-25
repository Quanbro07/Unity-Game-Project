using Pathfinding;
using System.Collections;
using UnityEngine;

public class BoarController : MonoBehaviour
{
    public AIPath aiPath;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AIDestinationSetter destinationSetter;

    #region Animation
    [SerializeField] private Animator animator;
    private int boarIdle = Animator.StringToHash("BoarIdle");
    private int boarRun = Animator.StringToHash("BoarRun");
    private int boarChillRun = Animator.StringToHash("BoarChillRun");
    private int currentAnimation;
    #endregion

    #region RandomWalk
    public enum State { Chill, Chase, GoToLastPos }
    private State currentState = State.Chill;
    [SerializeField] private float wanderRadius = 3f;
    [SerializeField] private float idleDelay = 2f; // thời gian chờ trước khi wander
    
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float RunSpeed = 3f;

    private Transform wanderTarget;
    private float wanderCooldown = 3f;
    private float wanderTimer;
    private bool isWaitingToWander = false; // flag để track trạng thái đang chờ
    #endregion

    private void Start()
    {
        wanderTarget = new GameObject("WanderTarget").transform;
    }

    private void Update()
    {
        if (currentState == State.Chill)
        {
            WanderLogic();
        }
        else if (currentState == State.GoToLastPos)
        {
            CheckReachedLastPos();
        }
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

        // Hướng ngang trái/phải → chạy và flip
        if (currentState == State.Chill)
        {
            currentAnimation = boarChillRun;
        }
        else if (currentState == State.GoToLastPos)
        {
            currentAnimation = boarRun; // Chạy nhanh khi đi tới vị trí cuối
        }
        else
        {
            currentAnimation = boarRun;
        }
        spriteRenderer.flipX = velocity.x > 0;
    }

    private void UpdateAnimation(int animation)
    {
        animator.CrossFade(animation, 0);
    }

    public void SetState(State newState)
    {
        currentState = newState;
        if (newState == State.Chase)
        {
            // Stop wandering logic khi chase
            StopAllCoroutines();
            isWaitingToWander = false;
            destinationSetter.target = null;
            aiPath.maxSpeed = RunSpeed;
            
        }
        else if (newState == State.GoToLastPos)
        {
            // Stop wandering logic và enable pathfinding để đi tới vị trí cuối
            StopAllCoroutines();
            isWaitingToWander = false;
            aiPath.enabled = true;
            destinationSetter.enabled = true;
            aiPath.maxSpeed = walkSpeed;

        }
        else if (newState == State.Chill)
        {
            wanderTimer = 0;

            // Disable pathfinding ban đầu
            aiPath.enabled = false;
            destinationSetter.enabled = false;

            // Bắt đầu delay trước khi wander
            if (!isWaitingToWander)
            {
                StartCoroutine(DelayBeforeWander());
            }
        }
    }

    public State GetState()
    {
        return currentState;
    }

    private void WanderLogic()
    {
        // Chỉ wander khi không đang trong trạng thái chờ
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

        // Chỉ bắt đầu wander nếu vẫn đang ở state Chill
        if (currentState == State.Chill)
        {
            isWaitingToWander = false;
            SetRandomWanderTarget();
            aiPath.enabled = true;
            destinationSetter.enabled = true;
        }
    }

    private void CheckReachedLastPos()
    {
        // Kiểm tra xem đã đến vị trí cuối chưa
        if (aiPath.reachedEndOfPath || aiPath.remainingDistance < 0.5f)
        {
            // Đã đến vị trí cuối, chuyển về state Chill để bắt đầu delay
            SetState(State.Chill);
        }
    }
}