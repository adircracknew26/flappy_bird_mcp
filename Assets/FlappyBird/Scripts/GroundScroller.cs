using UnityEngine;

/// <summary>Desplaza el suelo en bucle para dar sensación de movimiento.</summary>
public class GroundScroller : MonoBehaviour
{
    public float speed = 4.5f;
    public float width = 16f;
    public float wrapX = -24f;

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.State == GameState.GameOver) return;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform piece = transform.GetChild(i);
            piece.position += Vector3.left * speed * Time.deltaTime;

            if (piece.position.x <= wrapX)
                piece.position += Vector3.right * width * transform.childCount;
        }
    }
}
