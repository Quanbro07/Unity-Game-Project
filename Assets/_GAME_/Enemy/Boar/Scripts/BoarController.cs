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
    private bool isWaitingToWander = false;
    #endregion

    #region Health System
    [Header("Health System")]
    public float maxHealth = 20f; // Máu tối đa của Boar
    private float currentHealth;
    #endregion

    // =========================================================
    // VÙNG CODE ĐÃ SỬA ĐỔI: Sử dụng một mảng/danh sách các Prefab EXP Orb
    // =========================================================
    [Header("Experience Drop")]
    [SerializeField] private GameObject[] experienceOrbPrefabs;
    public float orbSpawnSpread = 0.5f;

    [System.Serializable]
    public struct DropChance
    {
        public GameObject orbPrefab;
        public int minAmount;
        public int maxAmount;
        [Range(0f, 1f)]
        public float dropRate;
    }

    [SerializeField] private DropChance[] orbDropChances;
    // =========================================================

    private void Start()
    {
        wanderTarget = new GameObject("WanderTarget").transform;
        currentHealth = maxHealth;
        // Gán AIPath nếu chưa có
        if (aiPath == null) aiPath = GetComponent<AIPath>();
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

        if (currentState == State.Chill)
        {
            currentAnimation = boarChillRun;
        }
        else if (currentState == State.GoToLastPos)
        {
            currentAnimation = boarRun;
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
            StopAllCoroutines();
            isWaitingToWander = false;
            aiPath.maxSpeed = RunSpeed;
        }
        else if (newState == State.GoToLastPos)
        {
            StopAllCoroutines();
            isWaitingToWander = false;
            aiPath.enabled = true;
            destinationSetter.enabled = true;
            aiPath.maxSpeed = walkSpeed;
        }
        else if (newState == State.Chill)
        {
            wanderTimer = 0;
            aiPath.enabled = false;
            destinationSetter.enabled = false;
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
        if (aiPath.reachedEndOfPath || aiPath.remainingDistance < 0.5f)
        {
            SetState(State.Chill);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        foreach (DropChance drop in orbDropChances)
        {
            if (Random.value <= drop.dropRate)
            {
                int amountToDrop = Random.Range(drop.minAmount, drop.maxAmount + 1);

                for (int i = 0; i < amountToDrop; i++)
                {
                    if (drop.orbPrefab != null)
                    {
                        Vector3 spawnPosition = transform.position + new Vector3(
                            Random.Range(-orbSpawnSpread, orbSpawnSpread),
                            Random.Range(-orbSpawnSpread, orbSpawnSpread),
                            0
                        );
                        Instantiate(drop.orbPrefab, spawnPosition, Quaternion.identity);
                    }
                    else
                    {
                        Debug.LogWarning("Experience Orb Prefab is not assigned for a drop chance in " + gameObject.name + "!");
                    }
                }
            }
        }

        Destroy(gameObject);
        Debug.Log(gameObject.name + " died and potentially dropped multiple types of experience orbs.");
    }

    void OnMouseDown()
    {
        TakeDamage(10f);
    }
}