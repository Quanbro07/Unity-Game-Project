using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    #region Movement
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 movement;
    private int moveRight = Animator.StringToHash("PlayerMoveRight");
    private int moveUp = Animator.StringToHash("PlayerMoveUp");
    private int dash = Animator.StringToHash("PlayerDash");
    #endregion

    #region Dash
    private bool canDash = true;
    private bool isDashing = false;
    [SerializeField] private float dashPower = 10f;
    [SerializeField] private float dashTime = 0.4f;
    [SerializeField] private float dashCoolDown = 0.1f;

    #endregion

    #region Animation
    private int animaitonChoice = Animator.StringToHash("PlayerDash");
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    #region Tick
    private void Update()
    {
        GatherInput();
        HandleDash();
    }
    #endregion

    #region FixedUpdate
    private void FixedUpdate()
    {
        UpdateMovement();
        UpdateDirection();
        
    }
    #endregion
    private void GatherInput()
    {

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");


    }

    private void HandleDash()
    {
        if (Input.GetMouseButtonDown(1) && canDash)
        {
            Debug.Log("Movement vector: " + rb.linearVelocity);
            UpdateAnimation(dash);
            StartCoroutine(Dash());
        }
    }

    private void UpdateMovement()
    {
        if (isDashing) return;

        rb.linearVelocity = movement.normalized * moveSpeed;
    }

    private void UpdateDirection()
    {
        if (isDashing) return;

        if (movement.x == -1) // Move Left
        {
            animaitonChoice = moveRight;
            spriteRenderer.flipX = true;
        }
        else if (movement.x == 1) // Move Right
        {
            spriteRenderer.flipX = false;
            animaitonChoice = moveRight;
        }

        if (movement.y == 1) // Move Up
        {
            animaitonChoice = moveUp;
        }
        else if (movement.y == -1) // Move Down
        {
            animaitonChoice = moveRight;
            
        }

       
        UpdateAnimation(animaitonChoice);
  

    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        rb.linearVelocity = movement.normalized * dashPower;
        yield return new WaitForSeconds(dashTime);

        isDashing = false;
        
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }

    private void UpdateAnimation(int animation)
    {
        animator.CrossFade(animation, 0);
    }
}
