using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Elena : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float movementSpeed;
    Vector2 moveInput;
    
    [Header("Jump")]
    [SerializeField] float jumpPower;
    bool onGround;

    [Header("Dash")]
    [SerializeField] float dashPower;
    [SerializeField] float dashTime = 1f;
    [SerializeField] float dashCooldown = 1.5f;
    [SerializeField] bool canDash;
    [SerializeField] bool isDashing;
    float dashCooldownCounter;

    [Header("Slide")]
    [SerializeField] float slidePower;
    [SerializeField] float slideTime;
    [SerializeField] float slideCooldown = 1.5f;
    [SerializeField] bool canSlide;
    [SerializeField] bool isSliding;
    float slideCooldownCounter;

    [Header("Attack")]
    [SerializeField] float movementFreezeTimer;
    [SerializeField] float attackLeapPower;
    [SerializeField] float attackLeapTime;
    bool isAttacking;

    Rigidbody2D rb;
    Animator anim;
    CapsuleCollider2D bodyCol;
    BoxCollider2D feetCol;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bodyCol = GetComponent<CapsuleCollider2D>();
        feetCol = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        canDash = true;
        canSlide = true;
    }

    private void Update()
    {
        if (!canDash)
        {
            dashCooldownCounter -= Time.deltaTime;
            if (dashCooldownCounter <= 0) canDash = true;
        }
        else if (!canSlide)
        {
            slideCooldownCounter -= Time.deltaTime;
            if (slideCooldownCounter <= 0) canSlide = true;
        }


        anim.SetBool("isAttacking", isAttacking);
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
        anim.SetBool("Fall", IsFalling());

    }

    #region Input System
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnAttack(InputValue value)
    {
        if (isDashing) anim.SetTrigger("DashAttack");
        else anim.SetTrigger("FirstAttack");
        StartCoroutine(AttackLeapCoroutine());
    }

    void OnJump(InputValue value)
    {
        bool onGround = feetCol.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (!onGround) return;
        if (value.isPressed)
        {
            Debug.Log("Pressed Jmup");
            rb.velocity = new(rb.velocity.x, jumpPower);
            anim.SetTrigger("Jump");
        }
    }

    void OnDash(InputValue value)
    {
        canDash = IsMoving() && onGround && !isSliding && dashCooldownCounter <= 0;
        if (value.isPressed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    void OnSlide(InputValue value)
    {
        canSlide = IsMoving() && onGround && !isDashing && slideCooldownCounter <= 0;
        if (value.isPressed && canSlide) 
        {
            StartCoroutine(SlideCoroutine());
        }
    }

    #endregion

    void Movement()
    {
        rb.velocity = new(moveInput.x * movementSpeed, rb.velocity.y);
        anim.SetBool("isRunning", IsMoving());
    }

    void Flip()
    {
        if (IsMoving()) transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
    }

    bool IsMoving()
    {
        return Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
    }

    bool IsFalling()
    {
        onGround = feetCol.IsTouchingLayers(LayerMask.GetMask("Ground"));
        return !onGround && rb.velocity.y <= Mathf.Epsilon;
    }

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
        movementFreezeTimer = .4f;
        Vector2 attackLeap = transform.localScale.x * attackLeapPower * transform.right;

        float attackLeapStartTime = Time.time;
        while (Time.time < attackLeapStartTime + attackLeapTime)
        {
            rb.velocity = Vector2.zero;
            transform.Translate(attackLeap * Time.deltaTime);
            yield return null;
        }
    }

}
