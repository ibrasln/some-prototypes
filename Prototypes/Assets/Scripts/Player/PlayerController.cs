using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    private Player player;
    [SerializeField] private float moveSpeed; // Maybe I can use scriptable object for movement
    
    [SerializeField] private float dashCooldownTimer;
    [SerializeField] private float dashDistance;
    bool isPlayerDashing;

    private WaitForFixedUpdate waitForFixedUpdate;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        waitForFixedUpdate = new();
    }

    private void Update()
    {
        MovementInput();
        PlayerDashCooldownTimer();
    }

    private void MovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector2 direction = new(horizontal, 0f);
        bool leftShiftButtonDown = Input.GetKeyDown(KeyCode.LeftShift);

        if(!leftShiftButtonDown)
        {
            player.movementEvent.CallMovementEvent(moveSpeed, direction);
        }
        else if (dashCooldownTimer <= 0)
        {
            PlayerDash((Vector3)direction);
        }
    }

    private void PlayerDash(Vector3 direction)
    {
        StartCoroutine(PlayerDashRoutine(direction));
    }

    private IEnumerator PlayerDashRoutine(Vector3 direction)
    {
        isPlayerDashing = true;

        Vector3 targetPosition = player.transform.position + (direction * dashDistance);

        while (Vector3.Distance(player.transform.position, targetPosition) > 0)
        {
            player.movementToPositionEvent.CallMovementToPositionEvent(moveSpeed, targetPosition, player.transform.position, direction, isPlayerDashing, false);

            yield return waitForFixedUpdate;
        }

        isPlayerDashing = false;
        dashCooldownTimer = 2f;
        player.transform.position = targetPosition;
    }

    private void PlayerDashCooldownTimer()
    {
        dashCooldownTimer -= Time.deltaTime;
    }
}
