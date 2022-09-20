using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Elena : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    Vector2 moveInput;
    [SerializeField] float jumpPower;

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
        Movement();
        Flip();
    }

    #region Input System
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnAttack(InputValue value)
    {
        anim.SetTrigger("Attack");
    }

    void OnJump(InputValue value)
    {
        bool onGround = feetCol.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (!onGround) return;
        if (value.isPressed) rb.velocity = new(rb.velocity.x, jumpPower);
    }
    #endregion

    void Movement()
    {
        rb.velocity = new(moveInput.x * movementSpeed, rb.velocity.y);
        anim.SetBool("isRunning", IsPlayerMoving());
    }

    void Flip()
    {
        if (IsPlayerMoving()) transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
    }

    bool IsPlayerMoving()
    {
        return Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
    }

}
