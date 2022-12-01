using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEvent : MonoBehaviour
{
    public event Action<MovementEvent, MovementEventArgs> OnMovement;

    public void CallMovementEvent(float moveSpeed, Vector2 direction)
    {
        OnMovement?.Invoke(this, new MovementEventArgs() { moveSpeed = moveSpeed, direction = direction });
    }
}

public class MovementEventArgs : EventArgs
{
    public float moveSpeed;
    public Vector2 direction;
}
