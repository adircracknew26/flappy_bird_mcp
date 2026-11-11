using UnityEngine;
using UnityEngine.UI;

/// <summary>Construye el HUD en tiempo de ejecución con uGUI y la fuente integrada de Unity.</summary>
public class FlappyUI : MonoBehaviour
{
    private Font font;
    private Text scoreText;
    private Text hintText;
    private GameObject gameOverPanel;
    private Text gameOverScoreText;
    private Text bestScoreText;

    public void Build()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            font = Font.CreateDynamicFontFromOSFont("Arial", 32);

        GameObject canvasObject = new GameObject(
            "HUD",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));

        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        scoreText = MakeText(canvasObject.transform, "Score", "0", 92, TextAnchor.UpperCenter);
        SetRect(scoreText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -110f), new Vector2(700f, 150f));
        AddOutline(scoreText, new Color(0.15f, 0.09f, 0.02f), new Vector2(3f, -3f));

        hintText = MakeText(canvasObject.transform, "Hint", "ESPACIO / CLIC para volar", 54, TextAnchor.MiddleCenter);
        SetRect(hintText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 130f), new Vector2(1400f, 100f));
        AddOutline(hintText, new Color(0.1f, 0.1f, 0.1f), new Vector2(2f, -2f));

        BuildGameOverPanel(canvasObject.transform);
        gameOverPanel.SetActive(false);
    }

    public void SetScore(int score)
    {
        if (scoreText != null) scoreText.text = score.ToString();
    }

    public void ShowReady(bool show)
    {
        if (hintText != null) hintText.gameObject.SetActive(show);
    }

    public void ShowGameOver(int score, int best)
    {
        if (gameOverPanel == null) return;

        gameOverPanel.SetActive(true);
        if (gameOverScoreText != null) gameOverScoreText.text = "Puntaje: " + score;
        if (bestScoreText != null) bestScoreText.text = "Mejor: " + best;
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    private void BuildGameOverPanel(Transform parent)
    {
        gameOverPanel = new GameObject(
            "GameOverPanel",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image));

        gameOverPanel.transform.SetParent(parent, false);

        RectTransform panelRect = gameOverPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image background = gameOverPanel.GetComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.55f);
        background.raycastTarget = false;

        Text title = MakeText(gameOverPanel.transform, "Title", "GAME OVER", 110, TextAnchor.MiddleCenter);
        SetRect(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 190f), new Vector2(1200f, 160f));
        title.color = new Color(1f, 0.84f, 0.2f);
        AddOutline(title, new Color(0.2f, 0.1f, 0.02f), new Vector2(4f, -4f));

        gameOverScoreText = MakeText(gameOverPanel.transform, "FinalScore", "Puntaje: 0", 64, TextAnchor.MiddleCenter);
        SetRect(gameOverScoreText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 50f), new Vector2(1000f, 100f));
        AddOutline(gameOverScoreText, new Color(0.08f, 0.08f, 0.08f), new Vector2(2f, -2f));

        bestScoreText = MakeText(gameOverPanel.transform, "BestScore", "Mejor: 0", 48, TextAnchor.MiddleCenter);
        SetRect(bestScoreText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, -30f), new Vector2(1000f, 90f));
        AddOutline(bestScoreText, new Color(0.08f, 0.08f, 0.08f), new Vector2(2f, -2f));

        Text restart = MakeText(gameOverPanel.transform, "RestartHint", "ESPACIO / CLIC para reiniciar", 42, TextAnchor.MiddleCenter);
        SetRect(restart.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, -160f), new Vector2(1200f, 80f));
        restart.color = new Color(0.9f, 0.95f, 1f);
        AddOutline(restart, new Color(0.08f, 0.08f, 0.08f), new Vector2(2f, -2f));
    }

    private Text MakeText(Transform parent, string name, string content, int size, TextAnchor anchor)
    {
        GameObject textObject = new GameObject(
            name,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Text));

        textObject.transform.SetParent(parent, false);

        Text text = textObject.GetComponent<Text>();
        text.font = font;
        text.text = content;
        text.fontSize = size;
        text.fontStyle = FontStyle.Bold;
        text.alignment = anchor;
        text.color = Color.white;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        return text;
    }

    private void SetRect(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size)
    {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private void AddOutline(Text text, Color color, Vector2 distance)
    {
        Outline outline = text.gameObject.AddComponent<Outline>();
        outline.effectColor = color;
        outline.effectDistance = distance;
        outline.useGraphicAlpha = true;
    }
}
