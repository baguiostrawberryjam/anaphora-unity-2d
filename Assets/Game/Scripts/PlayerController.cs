using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public bool hasFlashlight = false;
    public bool hasInteractedSwitch = false;
    public bool hasKey = false;
    public bool hasInteractedDrawer = false;
    public bool hasInteractedRef = false;
    public bool hasCheckedBulletinBoard = false;
    public bool canMove = true;

    public JoystickController joystick;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        MoveToSpawnPoint();
    }

    void Update()
    {
        if (!canMove)
        {
            moveInput = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 joystickInput = joystick != null ? joystick.InputDirection : Vector2.zero;

        if (joystickInput.magnitude > 0.1f)
        {
            moveInput = joystickInput;
        }
        else
        {
            if (moveX != 0f && moveY != 0f)
            {
                if (Mathf.Abs(moveX) >= Mathf.Abs(moveY))
                {
                    moveY = 0f;
                }
                else
                {
                    moveX = 0f;
                }
            }
            Vector2 keyboardInput = new Vector2(moveX, moveY);
            moveInput = keyboardInput.normalized;
        }

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
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            return;
        }

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    void MoveToSpawnPoint()
    {
        string spawnName = PlayerPrefs.GetString("SpawnPoint", "");

        if (!string.IsNullOrEmpty(spawnName))
        {
            GameObject spawnPoint = GameObject.Find(spawnName);

            if (spawnPoint != null)
            {
                transform.position = spawnPoint.transform.position;
            }
        }
    }
}