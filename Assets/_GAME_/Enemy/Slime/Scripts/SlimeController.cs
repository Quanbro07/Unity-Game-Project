using UnityEngine;

public class SlimeController : MonoBehaviour
{
    [SerializeField] private int damage = 3;
    [SerializeField] private Health health;

    #region Movement
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 movement = Vector2.zero;

    #endregion

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    #region Animation
    [SerializeField] private Animator animator;
    private int animaitonChoice = Animator.StringToHash("Slime_Sleep");

    private int slimeIdle = Animator.StringToHash("Slime_Idle");
    private int slimeWalkLeft = Animator.StringToHash("Slime_Walk_Left");
    private int slimeWalkUp = Animator.StringToHash("Slime_Walk_Up");
    #endregion
    
    private float threshold = 0.1f; // Ngưỡng tối thiểu để slime phản ứng

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void Update()
    {
        UpdateAnimation(animaitonChoice);

    }

    private void FixedUpdate()
    {
        UpdateMovement();
        UpdateDirection();

    }

    // Use For Test
    public void GatherInput(Vector2 move)
    {

        move.x = Mathf.Abs(move.x) > threshold ? Mathf.Sign(move.x) : 0;
        move.y = Mathf.Abs(move.y) > threshold ? Mathf.Sign(move.y) : 0;


        movement = move;
    }

    private void UpdateMovement()
    {
        rb.linearVelocity = movement.normalized * moveSpeed;
    }

    private void UpdateDirection()
    {
        // Not Moving
        if(movement.x  == 0 && movement.y == 0)
        {
            animaitonChoice = slimeIdle;
        }
        else // Is Moving
        {
            if(movement.x < 0) // Move Left
            {
                animaitonChoice = slimeWalkLeft;
                spriteRenderer.flipX = false;
            }
            else if(movement.x > 0) // Move Right
            {
                animaitonChoice = slimeWalkLeft;
                spriteRenderer.flipX = true;
            }

            if(movement.y > 0) // Move Up
            {
                animaitonChoice = slimeWalkUp;
            }
            else if(movement.y < 0)
            {
                animaitonChoice = slimeIdle;
            }
        }
    }

    private void UpdateAnimation(int animation)
    {
        animator.CrossFade(animation,0);
    }
}
