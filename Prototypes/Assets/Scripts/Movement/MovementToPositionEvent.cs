using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementToPositionEvent : MonoBehaviour
{
    public event Action<MovementToPositionEvent, MovementToPositionEventArgs> OnMovementToPosition;

    public void CallMovementToPositionEvent(float moveSpeed, Vector3 movePosition, Vector3 currentPosition, Vector2 direction, bool isDashing, bool isSliding)
    {
        OnMovementToPosition?.Invoke(this, new() { moveSpeed = moveSpeed, movePosition = movePosition, currentPosition = currentPosition, direction = direction, isDashing = isDashing, isSliding = isSliding });
    }
}

public class MovementToPositionEventArgs : EventArgs
{
    public float moveSpeed;
    public Vector3 movePosition;
    public Vector3 currentPosition;
    public Vector2 direction;
    public bool isDashing;
    public bool isSliding;
}
