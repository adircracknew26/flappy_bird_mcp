using UnityEngine;

/// <summary>Controla el vuelo, la rotación y la animación del pájaro.</summary>
[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class BirdController : MonoBehaviour
{
    [Header("Vuelo")]
    public float flapVelocity = 8.2f;
    public float gravityScale = 2.2f;
    public float maxFallSpeed = -11f;
    public float ceiling = 6.2f;

    [Header("Rotación")]
    public float upAngle = 25f;
    public float downAngle = -80f;
    public float rotationSpeed = 7f;

    [Header("Animación")]
    public float frameTime = 0.09f;
    public float bobAmplitude = 0.12f;
    public float bobSpeed = 3f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Sprite[] frames;
    private Vector3 startPosition;
    private float animTimer;
    private float bobTime;
    private int frameIndex;
    private bool flying;
    private bool dead;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        startPosition = transform.position;

        rb.freezeRotation = true;

        frames = new Sprite[3];
        frames[0] = Resources.Load<Sprite>("BirdFrames/bird_0");
        frames[1] = Resources.Load<Sprite>("BirdFrames/bird_1");
        frames[2] = Resources.Load<Sprite>("BirdFrames/bird_2");

        if (spriteRenderer != null && frames[1] != null)
            spriteRenderer.sprite = frames[1];
    }

    public void SetReady()
    {
        flying = false;
        dead = false;
        animTimer = 0f;
        bobTime = 0f;
        frameIndex = 0;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;

        if (spriteRenderer != null && frames[1] != null)
            spriteRenderer.sprite = frames[1];
    }

    public void StartFlying()
    {
        flying = true;
        dead = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = gravityScale;
        Flap();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.State == GameState.Ready)
        {
            bobTime += Time.deltaTime;
            transform.position = startPosition + Vector3.up * Mathf.Sin(bobTime * bobSpeed) * bobAmplitude;
            return;
        }

        if (!flying || dead) return;

        if (GameInput.PressedThisFrame())
            Flap();

        float verticalSpeed = rb.linearVelocity.y;
        float targetAngle = verticalSpeed > 1f ? upAngle : (verticalSpeed < -1f ? downAngle : 0f);
        float blend = 1f - Mathf.Exp(-rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(0f, 0f, targetAngle),
            blend);

        if (transform.position.y > ceiling)
        {
            transform.position = new Vector3(transform.position.x, ceiling, transform.position.z);
            if (rb.linearVelocity.y > 0f)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (!flying || dead) return;

        if (rb.linearVelocity.y < maxFallSpeed)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed);
    }

    public void Flap()
    {
        if (dead) return;

        rb.linearVelocity = new Vector2(0f, flapVelocity);

        animTimer = 0f;
        frameIndex = 0;
        if (spriteRenderer != null && frames[0] != null)
            spriteRenderer.sprite = frames[0];

        Sfx.PlayFlap();
    }

    public void Die()
    {
        dead = true;
        flying = false;

        rb.gravityScale = gravityScale;
        rb.linearVelocity = new Vector2(0f, Mathf.Max(rb.linearVelocity.y, 2f));
        transform.rotation = Quaternion.Euler(0f, 0f, -90f);
    }

    private void UpdateAnimation()
    {
        if (frames == null || frames.Length < 3 || frames[0] == null) return;

        animTimer += Time.deltaTime;
        if (animTimer < frameTime) return;

        animTimer = 0f;
        frameIndex = (frameIndex + 1) % frames.Length;
        if (frames[frameIndex] != null)
            spriteRenderer.sprite = frames[frameIndex];
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (flying && !dead && GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }
}
