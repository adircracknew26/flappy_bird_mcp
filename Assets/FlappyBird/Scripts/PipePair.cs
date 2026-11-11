using UnityEngine;

/// <summary>Mueve un par de tuberías, detecta el punto y se destruye fuera de pantalla.</summary>
public class PipePair : MonoBehaviour
{
    [Header("Partes del prefab")]
    public Transform topBody;
    public Transform topCap;
    public Transform bottomBody;
    public Transform bottomCap;

    [Header("Ajustes")]
    public float pipeWidth = 1.6f;
    public float capHeight = 0.5f;

    private BoxCollider2D scoreTrigger;
    private float speed = 4.5f;
    private bool scored;

    void Awake()
    {
        if (topBody == null) topBody = transform.Find("TopBody");
        if (topCap == null) topCap = transform.Find("TopCap");
        if (bottomBody == null) bottomBody = transform.Find("BottomBody");
        if (bottomCap == null) bottomCap = transform.Find("BottomCap");

        scoreTrigger = GetComponent<BoxCollider2D>();
        if (scoreTrigger != null)
        {
            scoreTrigger.isTrigger = true;
            scoreTrigger.usedByComposite = false;
        }
    }

    public void Configure(float gapCenterY, float gapSize, float groundY, float topY, float moveSpeed)
    {
        speed = moveSpeed;

        float gapTop = gapCenterY + gapSize * 0.5f;
        float gapBottom = gapCenterY - gapSize * 0.5f;

        ConfigureSegment(topBody, topCap, gapTop, topY, true);
        ConfigureSegment(bottomBody, bottomCap, groundY, gapBottom, false);

        if (scoreTrigger != null)
        {
            scoreTrigger.size = new Vector2(1.5f, gapSize);
            scoreTrigger.offset = new Vector2(0f, gapCenterY);
            scoreTrigger.enabled = true;
        }

        scored = false;
    }

    private void ConfigureSegment(Transform body, Transform cap, float fromY, float toY, bool capAtGap)
    {
        if (body == null) return;

        float height = Mathf.Max(0.1f, toY - fromY);
        float center = (fromY + toY) * 0.5f;

        body.localPosition = new Vector3(0f, center, 0f);
        body.localScale = new Vector3(pipeWidth, height, 1f);

        if (cap != null)
        {
            // La tapa va siempre en el extremo que da al hueco.
            float capY = capAtGap ? fromY + capHeight * 0.5f : toY - capHeight * 0.5f;
            cap.localPosition = new Vector3(0f, capY, 0f);
            cap.localScale = new Vector3(1.25f, 1f, 1f);
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.State == GameState.GameOver)
            return;

        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x < -14f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (scored) return;
        if (other.GetComponent<BirdController>() == null) return;

        scored = true;
        if (scoreTrigger != null) scoreTrigger.enabled = false;
        if (GameManager.Instance != null) GameManager.Instance.AddScore();
    }
}
