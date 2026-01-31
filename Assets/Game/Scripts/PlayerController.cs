using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

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
        // 1. Get Input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // 2. Kill Diagonal (Prioritize Horizontal)
        if (moveX != 0) moveY = 0;

        moveInput = new Vector2(moveX, moveY).normalized;

        // 3. ANIMATION LOGIC
        // Only update "FaceX" and "FaceY" if we are actually moving.
        // This acts as the "Memory" for the Idle state.
        if (moveInput != Vector2.zero)
        {
            animator.SetFloat("FaceX", moveInput.x);
            animator.SetFloat("FaceY", moveInput.y);
        }

        // Always update speed so we know when to switch between Idle and Walk
        animator.SetFloat("Speed", moveInput.sqrMagnitude);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}