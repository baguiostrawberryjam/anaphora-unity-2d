using UnityEngine;

public class FlashlightAim : MonoBehaviour
{
    [Header("References")]
    public JoystickController aimJoystick;

    [Header("Aiming Settings")]
    public float angleOffset = -90f; // Kept at -90 based on our previous fix!

    void Update()
    {
        // 1. Get raw keyboard input exactly like the player does
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // 2. Get joystick input exactly like the player does
        Vector2 joystickInput = aimJoystick != null ? aimJoystick.InputDirection : Vector2.zero;

        Vector2 finalAim = Vector2.zero;

        // 3. EXACT COPY of your PlayerController prioritization logic
        if (joystickInput.magnitude > 0.1f)
        {
            finalAim = joystickInput;
        }
        else
        {
            if (moveX != 0f && moveY != 0f)
            {
                // This >= ensures Horizontal takes priority, just like the player!
                if (Mathf.Abs(moveX) >= Mathf.Abs(moveY))
                {
                    moveY = 0f;
                }
                else
                {
                    moveX = 0f;
                }
            }
            finalAim = new Vector2(moveX, moveY).normalized;
        }

        // 4. Apply rotation if we have an active input direction
        if (finalAim != Vector2.zero)
        {
            float angle = Mathf.Atan2(finalAim.y, finalAim.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + angleOffset);
        }
    }
}