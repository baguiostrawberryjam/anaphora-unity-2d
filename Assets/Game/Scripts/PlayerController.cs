using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public bool isKeyCollected;

    public JoystickController joystick;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 keyboardInput = new Vector2(moveX, moveY);
        Vector2 joystickInput = joystick != null ? joystick.InputDirection : Vector2.zero;

        if (joystickInput.magnitude > 0.1f)
        {
            moveInput = joystickInput;
        }
        else
        {
            if (moveX != 0) moveY = 0;
            moveInput = keyboardInput.normalized;
        }

        // Prevents diagonal speed boost
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        if (moveInput != Vector2.zero)
        {
            animator.SetFloat("FaceX", moveInput.x);
            animator.SetFloat("FaceY", moveInput.y);
        }

        animator.SetFloat("Speed", moveInput.sqrMagnitude);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}