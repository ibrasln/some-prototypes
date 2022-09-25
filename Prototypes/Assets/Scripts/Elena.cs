using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Elena : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float movementSpeed;
    Vector2 moveInput;
    bool isFacingRight;

    [Header("Jump")]
    [SerializeField] float jumpPower;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;
    [SerializeField] bool onGround;
    [SerializeField] int amountOfJumps = 2;
    [SerializeField] Vector2 wallJumpDirection;
    [SerializeField] float wallJumpForce;
    [SerializeField] float movementForceInAir;
    [SerializeField] float airDragMultiplier;
    float amountOfJumpsLeft;
    bool canJump;

    [Header("Dash")]
    [SerializeField] float dashPower;
    [SerializeField] float dashTime = 1f;
    [SerializeField] float dashCooldown = 1.5f;
    bool isDashing;
    float dashCooldownCounter;
    bool canDash;

    [Header("Slide")]
    [SerializeField] float slidePower;
    [SerializeField] float slideTime;
    [SerializeField] float slideCooldown = 1.5f;
    float slideCooldownCounter;
    bool canSlide;
    bool isSliding;

    [Header("Attack")]
    [SerializeField] float attackLeapPower;
    [SerializeField] float attackLeapTime;
    [SerializeField] float secondAttackCooldown = .35f;
    [SerializeField] float attackCooldown;
    float movementFreezeTimer;
    float secondAttackCooldownCounter;
    float attackCooldownCounter;
    int attackAmountLeft;
    bool isAttacking;

    [Header("Wall Slide")]
    [SerializeField] Transform wallCheck;
    [SerializeField] float wallCheckDistance;
    [SerializeField] float wallSlideSpeed;
    [SerializeField] bool onWall;
    [SerializeField] bool isWallSliding;

    [Header("Edge Grab")]
    [SerializeField] Transform edgeCheck;
    [SerializeField] float edgeCheckDistance;
    [SerializeField] bool onEdge;
    [SerializeField] bool isEdgeGrabing;

    Rigidbody2D rb;
    Animator anim;
    CapsuleCollider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        canDash = true;
        canSlide = true;
        attackAmountLeft = 1;
        wallJumpDirection.Normalize();
    }

    private void Update()
    {
        CheckIfCanDash();
        CheckIfCanSlide();
        CheckIfCanAttack();

        if (isAttacking)
        {
            movementFreezeTimer -= Time.deltaTime;
            if (movementFreezeTimer > 0)
            {
                return;
            }
            else isAttacking = false;
        }

        Movement();
        Flip();
        CheckIfCanJump();
        CheckIfCanWallSliding();
        CheckIfCanEdgeGrab();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        CheckSurroundings();
    }

    void UpdateAnimations()
    {
        anim.SetBool("isRunning", IsRunning());
        anim.SetBool("onGround", onGround);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isEdgeGrabing", isEdgeGrabing);
    }

    void CheckSurroundings()
    {
        onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        onWall = Physics2D.Raycast(wallCheck.position, transform.localScale.x * transform.right, wallCheckDistance, whatIsGround);
        onEdge = !Physics2D.Raycast(edgeCheck.position, transform.localScale.x * transform.right, edgeCheckDistance, whatIsGround); // When ray isn't touching the wall.
    }

    #region Input System
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnAttack(InputValue value)
    {
        if (!onGround || attackCooldownCounter > 0) return;

        if (value.isPressed)
        {
            if (isDashing) anim.SetTrigger("DashAttack");
            else
            {
                secondAttackCooldownCounter = secondAttackCooldown;
                attackCooldownCounter = attackCooldown;
                Debug.Log(attackAmountLeft);
                anim.SetTrigger("Attack");
                anim.SetFloat("attackAmount", attackAmountLeft);
                attackAmountLeft++;
                if (attackAmountLeft > 2) attackAmountLeft = 1; 
            }
            StartCoroutine(AttackLeapCoroutine());
        }
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed && canJump)
        {
            anim.ResetTrigger("Attack");
            anim.ResetTrigger("DashAttack");

            if (!isWallSliding)
            {
                Debug.Log("Jump from Ground");
                rb.velocity = new(rb.velocity.x, jumpPower);
            }
            else if ((onWall || isWallSliding) && !onGround && moveInput.x != 0)
            {
                if (moveInput.x != 1 && isFacingRight)
                {
                    Vector2 forceToAdd = new(wallJumpForce * wallJumpDirection.x * -2, wallJumpForce * wallJumpDirection.y);
                    rb.AddForce(forceToAdd, ForceMode2D.Impulse);
                    isWallSliding = false;
                }
                else if (moveInput.x != -1 && !isFacingRight)
                {
                    Vector2 forceToAdd = new(wallJumpForce * wallJumpDirection.x * 2, wallJumpForce * wallJumpDirection.y);
                    rb.AddForce(forceToAdd, ForceMode2D.Impulse);
                    isWallSliding = false;
                }
            }
            else if (isEdgeGrabing)
            {
                Vector2 forceToAdd = new(wallJumpForce * wallJumpDirection.x, wallJumpForce * wallJumpDirection.y);
                rb.AddForce(forceToAdd, ForceMode2D.Impulse);
                isEdgeGrabing = false;
            }

            amountOfJumpsLeft--;
        }
    }

    void OnDash(InputValue value)
    {
        canDash = IsRunning() && onGround && !isSliding && dashCooldownCounter <= 0;
        if (value.isPressed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    void OnSlide(InputValue value)
    {
        canSlide = IsRunning() && onGround && !isDashing && slideCooldownCounter <= 0;
        if (value.isPressed && canSlide) 
        {
            StartCoroutine(SlideCoroutine());
        }
    }
    #endregion

    #region Check If Can ...
    void CheckIfCanAttack()
    {
        attackCooldownCounter -= Time.deltaTime;

        secondAttackCooldownCounter -= Time.deltaTime;
        if (secondAttackCooldownCounter <= 0)
        {
            attackAmountLeft = 1;
        } 
    }

    void CheckIfCanJump()
    {
        if (onGround && rb.velocity.y <= .1f)
        {
            amountOfJumpsLeft = amountOfJumps;
        }
        else if (isWallSliding && rb.velocity.y <= .1f)
        {
            amountOfJumpsLeft = 1;
        }

        if (amountOfJumpsLeft <= 0) canJump = false;
        else canJump = true;

    }

    void CheckIfCanDash()
    {
        if (!canDash)
        {
            dashCooldownCounter -= Time.deltaTime;
            if (dashCooldownCounter <= 0)
            {
                canDash = true;
            }
        }
    }

    void CheckIfCanSlide()
    {
        if (!canSlide)
        {
            slideCooldownCounter -= Time.deltaTime;
            if (slideCooldownCounter <= 0)
            {
                canSlide = true;
            }
        }
    }

    void CheckIfCanWallSliding()
    {
        if (onWall && !onGround && !isEdgeGrabing)
        {
            isWallSliding = true;
        }
        else isWallSliding = false;
    }

    void CheckIfCanEdgeGrab()
    {
        if (onWall && onEdge && !onGround)
        {
            rb.gravityScale = 0;
            isEdgeGrabing = true;
        }
        else
        {
            rb.gravityScale = 5;
            isEdgeGrabing = false;
        }
    }
    #endregion

    void Movement()
    {
        if (onGround) rb.velocity = new(moveInput.x * movementSpeed, rb.velocity.y);

        else if (isEdgeGrabing) return;
        
        else if (isWallSliding && rb.velocity.y < -wallSlideSpeed) rb.velocity = new(rb.velocity.x, -wallSlideSpeed);
        
        else if (!onGround && !isWallSliding && !isEdgeGrabing && moveInput.x != 0)
        {
            Vector2 forceToAdd = new(movementForceInAir * moveInput.x, 0);
            rb.AddForce(forceToAdd);

            if (Mathf.Abs(rb.velocity.x) > movementSpeed)
            {
                rb.velocity = new Vector2(movementSpeed * moveInput.x, rb.velocity.y);
            }
        }
        
        else if (!onGround && !isWallSliding && !isEdgeGrabing && moveInput.x == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }

        Debug.Log(rb.velocity);
    }

    void Flip()
    {
        if (!isWallSliding && !isEdgeGrabing)
        {
            if (moveInput.x < 0)
            {
                isFacingRight = false;
                transform.localScale = new(-1, 1, 1);
            }
            else if (moveInput.x > 0)
            {
                isFacingRight = true;
                transform.localScale = new(1, 1, 1);
            }
        }
    }

    bool IsRunning()
    {
        return Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
    }

    #region Coroutines
    IEnumerator DashCoroutine()
    {
        isDashing = true;
        canDash = false;
        dashCooldownCounter = dashCooldown;

        anim.SetTrigger("Dash");
        Vector2 dashSpeed = transform.localScale.x * dashPower * transform.right;

        float dashStartTime = Time.time;
        while (Time.time < dashStartTime + dashTime)
        {
            transform.Translate(dashSpeed * Time.deltaTime);
            yield return null;
        }
        isDashing = false;
    }

    IEnumerator SlideCoroutine()
    {
        isSliding = true;
        canSlide = false;
        slideCooldownCounter = slideCooldown;
        
        anim.SetTrigger("Slide");
        Vector2 slideSpeed = transform.localScale.x * slidePower * transform.right;

        float slideStartTime = Time.time;
        while (Time.time < slideStartTime + slideTime)
        {
            transform.Translate(slideSpeed * Time.deltaTime);
            yield return null;
        }
        isSliding = false;
    }

    IEnumerator AttackLeapCoroutine()
    {
        isAttacking = true;
        movementFreezeTimer = .35f;
        Vector2 attackLeap = transform.localScale.x * attackLeapPower * transform.right;

        float attackLeapStartTime = Time.time;
        while (Time.time < attackLeapStartTime + attackLeapTime)
        {
            rb.velocity = new(0f, rb.velocity.y);
            transform.Translate(attackLeap * Time.deltaTime);
            yield return null;
        }
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector2((wallCheck.position.x + wallCheckDistance) * transform.localScale.x, wallCheck.position.y));
        Gizmos.DrawLine(edgeCheck.position, new Vector2((edgeCheck.position.x + edgeCheckDistance) * transform.localScale.x, edgeCheck.position.y));
    }
}
