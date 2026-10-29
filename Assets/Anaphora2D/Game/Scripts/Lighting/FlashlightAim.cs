using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightAim : MonoBehaviour
{
    [Header("References")]
    public JoystickController aimJoystick;
    public PlayerController playerController;

    [Header("Aiming Settings")]
    public float angleOffset = -90f;

    private Light2D flashlight;

    void Start()
    {
        flashlight = GetComponent<Light2D>();
    }

    void Update()
    {
        if (playerController == null || !PlayerController.hasFlashlight)
        {
            if (flashlight != null) flashlight.enabled = false;
            return;
        }

        if (flashlight != null) flashlight.enabled = true;

        if (!playerController.canMove)
            return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 joystickInput = aimJoystick != null ? aimJoystick.InputDirection : Vector2.zero;
        Vector2 finalAim = Vector2.zero;

        if (joystickInput.magnitude > 0.1f)
        {
            finalAim = joystickInput;
        }
        else
        {
            if (moveX != 0f && moveY != 0f)
            {
                if (Mathf.Abs(moveX) >= Mathf.Abs(moveY))
                    moveY = 0f;
                else
                    moveX = 0f;
            }
            finalAim = new Vector2(moveX, moveY).normalized;
        }

        if (finalAim != Vector2.zero)
        {
            float angle = Mathf.Atan2(finalAim.y, finalAim.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + angleOffset);
        }
    }
}