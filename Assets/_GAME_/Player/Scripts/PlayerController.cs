using System;
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
    private Vector2 lastDirection = Vector2.zero;
    #endregion

    #region Dash
    private bool canDash = true;
    private bool isDashing = false;
    [SerializeField] private float dashPower = 10f;
    [SerializeField] private float dashTime = 0.4f;
    [SerializeField] private float dashCoolDown = 0.1f;

    #endregion

    #region Animation
    private int animaitonChoice = Animator.StringToHash("PlayerIdleRight");

    private int playerMoveRight = Animator.StringToHash("PlayerMoveRight");
    private int playerMoveUp = Animator.StringToHash("PlayerMoveUp");
    private int playerDash = Animator.StringToHash("PlayerDash");

    private int playerIdleRight = Animator.StringToHash("PlayerIdleRight");
    private int playerIdleUp = Animator.StringToHash("PlayerIdleUp");
    private int playerIdleDown = Animator.StringToHash("PlayerIdleDown");
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
        if (Input.GetMouseButtonDown(1) && canDash && movement != Vector2.zero)
        {
            Debug.Log("Movement vector: " + rb.linearVelocity);
            UpdateAnimation(playerDash);
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

        // Not Moving
        if(movement.x == 0 && movement.y == 0)
        {   
            if(lastDirection != Vector2.zero && lastDirection == Vector2.up)
            {
                animaitonChoice = playerIdleUp;
            }
            else
            {
                animaitonChoice = playerIdleRight;
            }
        }
        else // Moving
        {
            if (movement.x == -1) // Move Left
            {
                animaitonChoice = playerMoveRight;
                spriteRenderer.flipX = true;
            }
            else if (movement.x == 1) // Move Right
            {
                spriteRenderer.flipX = false;
                animaitonChoice = playerMoveRight;
            }

            if (movement.y == 1) // Move Up
            {
                animaitonChoice = playerMoveUp;
            }
            else if (movement.y == -1) // Move Down
            {
                animaitonChoice = playerMoveRight;

            }

            lastDirection = movement;
        }

        Debug.Log(lastDirection);

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
