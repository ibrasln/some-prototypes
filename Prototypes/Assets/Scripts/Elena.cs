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

    [Header("Dash")]
    [SerializeField] float dashPower;
    [SerializeField] float dashTime = 1f;
    bool isDashing;

    [Header("Attack")]
    bool isAttacking;
    float movementFreezeTimer;
    public float attackLeapPower;
    public float attackLeapTime;

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

    private void Update()
    {
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

    void OnSecondAttack(InputValue value)
    {
        anim.SetTrigger("SecondAttack");
        StartCoroutine(AttackLeapCoroutine());
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
        if (value.isPressed && IsMoving())
        {
            StartCoroutine(DashCoroutine());
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
        bool onGround = feetCol.IsTouchingLayers(LayerMask.GetMask("Ground"));
        return !onGround && rb.velocity.y <= Mathf.Epsilon;
    }

    IEnumerator DashCoroutine()
    {
        isDashing = true;
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

    IEnumerator AttackLeapCoroutine()
    {
        isAttacking = true;
        movementFreezeTimer = .75f;
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
