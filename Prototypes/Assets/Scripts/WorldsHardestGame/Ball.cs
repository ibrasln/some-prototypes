using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Vector2 topCorner;
    [SerializeField] private Vector2 bottomCorner;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private bool goTopCorner;

    private void Update()
    {
        if (Vector2.Distance(transform.position, topCorner) <= 0f)
        {
            goTopCorner = false;
        }
        else if (Vector2.Distance(transform.position, bottomCorner) <= 0f)
        {
            goTopCorner = true;
        }

        if (goTopCorner)
        {
            transform.position = Vector2.MoveTowards(transform.position, topCorner, moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, bottomCorner, moveSpeed * Time.deltaTime);
        }
    }
}
