using UnityEngine;

/// <summary>Genera pares de tuberías con un hueco aleatorio.</summary>
public class PipeSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject pipePrefab;

    [Header("Configuración")]
    public float spawnInterval = 1.55f;
    public float pipeSpeed = 4.5f;
    public float spawnX = 10.5f;
    public float gapSize = 3.4f;
    public float minGapCenter = -1.8f;
    public float maxGapCenter = 2.6f;
    public float groundY = -5f;
    public float topY = 7f;

    private float timer;
    private bool spawning;

    public void StartSpawning()
    {
        spawning = true;
        timer = 0.35f;
    }

    public void StopSpawning()
    {
        spawning = false;
    }

    void Update()
    {
        if (!spawning) return;
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        SpawnPipe();
        timer = spawnInterval;
    }

    private void SpawnPipe()
    {
        if (pipePrefab == null)
        {
            Debug.LogError("PipeSpawner: falta asignar el prefab de tuberías.");
            spawning = false;
            return;
        }

        float gapCenter = Random.Range(minGapCenter, maxGapCenter);
        GameObject instance = Instantiate(pipePrefab, new Vector3(spawnX, 0f, 0f), Quaternion.identity);
        PipePair pair = instance.GetComponent<PipePair>();

        if (pair != null)
            pair.Configure(gapCenter, gapSize, groundY, topY, pipeSpeed);
        else
            Debug.LogError("PipeSpawner: el prefab no tiene el componente PipePair.");
    }
}
