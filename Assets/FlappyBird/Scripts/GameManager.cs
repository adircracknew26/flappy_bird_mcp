using UnityEngine;

public enum GameState
{
    Ready,
    Playing,
    GameOver
}

/// <summary>Controla el estado de la partida, el puntaje y el reinicio.</summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; } = GameState.Ready;
    public int Score { get; private set; }
    public bool IsPlaying => State == GameState.Playing;

    [Header("Referencias")]
    [SerializeField] private BirdController bird;
    [SerializeField] private PipeSpawner spawner;

    private FlappyUI ui;
    private float gameOverAt;

    private const string BestScoreKey = "FlappyBird_BestScore";

    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;

        if (bird == null) bird = FindFirstObjectByType<BirdController>();
        if (spawner == null) spawner = FindFirstObjectByType<PipeSpawner>();

        ui = gameObject.AddComponent<FlappyUI>();
        ui.Build();
    }

    void Start()
    {
        ui.SetScore(0);
        ui.ShowReady(true);
        ui.HideGameOver();
        bird.SetReady();
        spawner.StopSpawning();
    }

    void Update()
    {
        if (State == GameState.Ready && GameInput.PressedThisFrame())
        {
            StartGame();
        }
        else if (State == GameState.GameOver &&
                 Time.unscaledTime - gameOverAt > 0.75f &&
                 GameInput.PressedThisFrame())
        {
            Restart();
        }
    }

    public void StartGame()
    {
        if (State != GameState.Ready) return;

        State = GameState.Playing;
        ui.ShowReady(false);
        bird.StartFlying();
        spawner.StartSpawning();
    }

    public void AddScore()
    {
        if (State != GameState.Playing) return;

        Score++;
        ui.SetScore(Score);
        Sfx.PlayScore();
    }

    public void GameOver()
    {
        if (State != GameState.Playing) return;

        State = GameState.GameOver;
        gameOverAt = Time.unscaledTime;

        spawner.StopSpawning();
        bird.Die();

        int best = PlayerPrefs.GetInt(BestScoreKey, 0);
        if (Score > best)
        {
            best = Score;
            PlayerPrefs.SetInt(BestScoreKey, best);
            PlayerPrefs.Save();
        }

        ui.ShowGameOver(Score, best);
        Sfx.PlayHit();
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
