using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementToPositionEvent))]
[DisallowMultipleComponent]
public class MovementToPosition : MonoBehaviour
{
    Rigidbody2D rb;
    private MovementToPositionEvent movementToPositionEvent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    private void OnEnable()
    {
        movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }

    private void OnDisable()
    {
        movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionEventArgs movementToPositionEventArgs)
    {
        MoveRigidbody(movementToPositionEventArgs.moveSpeed, movementToPositionEventArgs.currentPosition, movementToPositionEventArgs.movePosition);
    }

    private void MoveRigidbody(float moveSpeed, Vector3 currentPosition, Vector3 movePosition)
    {
        Vector2 unitVector = Vector3.Normalize(movePosition - currentPosition);
        rb.MovePosition(rb.position + (moveSpeed * Time.fixedDeltaTime * unitVector));
    }
}
