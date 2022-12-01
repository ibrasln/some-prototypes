using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(MovementEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    [HideInInspector] public MovementEvent movementEvent;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;

    private void Awake()
    {
        movementEvent = GetComponent<MovementEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }
}
