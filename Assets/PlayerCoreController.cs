using UnityEngine;

// Bắt buộc GameObject phải có Rigidbody2D
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCoreController : MonoBehaviour
{
    //          MOVE 
    [Header("Move")]
    [Tooltip("Tốc độ chạy trái/phải")]
    public float moveSpeed = 8f;

    //          DASH 
    [Header("Dash")]
    [Tooltip("Tốc độ khi dash")]
    public float dashSpeed = 20f;

    [Tooltip("Thời gian dash (giây)")]
    public float dashDuration = 0.15f;

    [Tooltip("Thời gian chờ giữa 2 lần dash (giây)")]
    public float dashCooldown = 0.3f;

    [Header("Combat / I-Frame")]
    [Tooltip("Đang dash thì nhân vật được coi là vô địch (cho hệ thống combat sau này)")]
    public bool isInvincibleWhileDash = true;

    //           BIẾN NỘI BỘ 
    Rigidbody2D rb;      // tham chiếu tới Rigidbody2D trên Player
    Animator anim;       // Animator (lấy từ con, ví dụ HorseRoot)

    float horizontalInput; // giá trị -1 / 0 / 1 từ phím A/D hoặc mũi tên
    bool isFacingRight = true; // đang quay mặt sang phải không?

    bool isDashing = false;    // có đang dash không?
    float dashTimer = 0f;      // đếm ngược thời gian dash
    float lastDashTime = -999; // thời điểm dash lần trước

    // Cho script khác đọc nếu cần 
    public bool IsDashing => isDashing;
    public bool IsInvincible => isDashing && isInvincibleWhileDash;

    void Awake()
    {
        // Lấy Rigidbody2D trên chính object này (Player)
        rb = GetComponent<Rigidbody2D>();

        // Lấy Animator từ con (HorseRoot / model)
        // => rất quan trọng vì Animator thường không nằm trực tiếp trên Player
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // 1) Lấy input ngang
        HandleInput();

        // 2) Xoay mặt nhân vật theo hướng input
        HandleFlip();

        // 3) Quản lý thời gian dash (đếm ngược)
        UpdateDashTimer();

        // 4) Cập nhật parameter cho Animator
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        // Di chuyển vật lý: nằm trong FixedUpdate cho mượt
        HandleMove();
    }

    //              INPUT  
    void HandleInput()
    {
        // Đọc input ngang từ Input Manager cũ (Horizontal axis)
        // A / LeftArrow => -1, D / RightArrow => +1
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Nhấn Shift trái để dash
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TryDash();
        }
    }

    //              MOVE   
    void HandleMove()
    {
        if (isDashing)
        {
            // Nếu đang dash: khóa y = 0, chỉ lao theo hướng mặt
            float dir = isFacingRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(dir * dashSpeed, 0f);
        }
        else
        {
            // Không dash: di chuyển bình thường, giữ nguyên vận tốc rơi (y)
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    //              FLIP HƯỚNG     
    void HandleFlip()
    {
        // Nếu có input > 0.1 và đang quay trái -> lật sang phải
        if (horizontalInput > 0.1f && !isFacingRight)
        {
            Flip();
        }
        // Nếu input < -0.1 và đang quay phải -> lật sang trái
        else if (horizontalInput < -0.1f && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        // Lật scale X của Player (tất cả con, bao gồm sprite, sẽ quay theo)
        Vector3 s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    //               DASH  
    void TryDash()
    {
        // Đang dash thì không dash lại
        if (isDashing) return;

        // Chưa hết cooldown thì không dash
        if (Time.time < lastDashTime + dashCooldown) return;

        // Bắt đầu dash
        isDashing = true;
        dashTimer = dashDuration;
        lastDashTime = Time.time;
    }

    void UpdateDashTimer()
    {
        if (!isDashing) return;

        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
        {
            isDashing = false;
        }
    }

    // Hủy dash nếu đụng tường / enemy (tuỳ game bạn muốn)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDashing) return;

        if (collision.collider.CompareTag("Wall") ||
            collision.collider.CompareTag("Enemy"))
        {
            isDashing = false;
        }
    }

    //           ANIMATOR   
    void UpdateAnimator()
    {
        if (anim == null) return;

        // Speed: dùng để Idle <-> Move
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        // IsDashing: dùng cho anim dash (nếu muốn)
        anim.SetBool("IsDashing", isDashing);

        // Không dùng IsGrounded, không có Jump nữa
    }
}
