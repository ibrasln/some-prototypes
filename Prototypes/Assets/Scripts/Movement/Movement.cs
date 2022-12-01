using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementEvent))]
public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private MovementEvent movementEvent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movementEvent = GetComponent<MovementEvent>();
    }

    private void OnEnable()
    {
        movementEvent.OnMovement += MovementEvent_OnMovement;
    }

    private void OnDisable()
    {
        movementEvent.OnMovement -= MovementEvent_OnMovement;
    }

    private void MovementEvent_OnMovement(MovementEvent movementEvent, MovementEventArgs movementEventArgs)
    {
        MoveRigidbody(movementEventArgs.moveSpeed, movementEventArgs.direction);
    }

    private void MoveRigidbody(float moveSpeed, Vector2 moveDirection)
    {
        rb.velocity = moveDirection * moveSpeed;
    }
}
