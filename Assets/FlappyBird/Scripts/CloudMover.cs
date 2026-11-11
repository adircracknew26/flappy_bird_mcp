using UnityEngine;

/// <summary>Mueve las nubes lentamente y las recicla al salir de pantalla.</summary>
public class CloudMover : MonoBehaviour
{
    public float speed = 0.4f;
    public float minX = -15f;
    public float maxX = 15f;

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x < minX)
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
    }
}
