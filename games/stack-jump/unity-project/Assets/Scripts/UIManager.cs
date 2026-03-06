using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Manages all UI: Menu panel, HUD (score), GameOver panel,
/// Perfect/Combo popups, and star rating.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestText;

    [Header("Menu Panel")]
    public GameObject menuPanel;
    public TextMeshProUGUI menuBestScoreText;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI gameOverBestText;
    public TextMeshProUGUI gameOverNewBestText;
    public TextMeshProUGUI starsText;            // Shows ⭐ ⭐⭐ ⭐⭐⭐

    [Header("Popup (auto-created if empty)")]
    public TextMeshProUGUI popupText;            // Floating "PERFECT!" / "COMBO x3!"

    // ── Lifecycle ──────────────────────────────────────────────────────────

    void OnEnable()
    {
        GameManager.OnGameStart       += HandleGameStart;
        GameManager.OnGameOver        += HandleGameOver;
        ScoreManager.OnScoreChanged   += UpdateHUDScore;
        ScoreManager.OnBestChanged    += HandleNewBest;
        StackController.OnBlockPlaced += HandleBlockPlaced;
    }

    void OnDisable()
    {
        GameManager.OnGameStart       -= HandleGameStart;
        GameManager.OnGameOver        -= HandleGameOver;
        ScoreManager.OnScoreChanged   -= UpdateHUDScore;
        ScoreManager.OnBestChanged    -= HandleNewBest;
        StackController.OnBlockPlaced -= HandleBlockPlaced;
    }

    void Start()
    {
        if (menuPanel)           menuPanel.SetActive(true);
        if (gameOverPanel)       gameOverPanel.SetActive(false);
        if (scoreText)           scoreText.gameObject.SetActive(false);
        if (bestText)            bestText.gameObject.SetActive(false);
        if (gameOverNewBestText) gameOverNewBestText.gameObject.SetActive(false);

        int best = ScoreManager.Instance ? ScoreManager.Instance.BestScore : 0;
        if (menuBestScoreText) menuBestScoreText.text = best > 0 ? $"Best: {best}" : "";

        EnsurePopupText();

        if (menuPanel == null) GameManager.Instance?.StartGame();
    }

    // ── Event handlers ─────────────────────────────────────────────────────

    void HandleGameStart()
    {
        if (menuPanel)     menuPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (scoreText)     { scoreText.gameObject.SetActive(true); scoreText.text = "0"; }
        if (bestText)      { bestText.gameObject.SetActive(true);  bestText.text = $"Best: {ScoreManager.Instance.BestScore}"; }
        if (gameOverNewBestText) gameOverNewBestText.gameObject.SetActive(false);
    }

    void HandleGameOver()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
        int score = ScoreManager.Instance ? ScoreManager.Instance.CurrentScore : 0;
        int best  = ScoreManager.Instance ? ScoreManager.Instance.BestScore    : 0;
        if (gameOverScoreText) gameOverScoreText.text = score.ToString();
        if (gameOverBestText)  gameOverBestText.text  = $"Best: {best}";

        // Star rating
        int stars = score >= 25 ? 3 : score >= 10 ? 2 : 1;
        if (starsText) starsText.text = new string('\u2605', stars) + new string('\u2606', 3 - stars);
    }

    void UpdateHUDScore(int score)
    {
        if (scoreText) scoreText.text = score.ToString();
        // Punch scale on score text
        if (scoreText) StartCoroutine(PunchScale(scoreText.transform, 1.35f, 0.12f));
    }

    void HandleNewBest(int best)
    {
        if (bestText) bestText.text = $"Best: {best}";
        if (gameOverNewBestText) gameOverNewBestText.gameObject.SetActive(true);
    }

    void HandleBlockPlaced(bool isPerfect, int combo)
    {
        if (!isPerfect) return;

        string msg = combo >= 3 ? $"COMBO x{combo}!" : "PERFECT!";
        Color  col = combo >= 3
            ? new Color(1f, 0.85f, 0.1f)   // gold for combo
            : new Color(0.3f, 1f, 0.5f);   // green for perfect

        ShowPopup(msg, col);
    }

    // ── Button callbacks ───────────────────────────────────────────────────

    public void OnPlayButton()  => GameManager.Instance.StartGame();
    public void OnRetryButton() => GameManager.Instance.RestartGame();

    // ── Popup helpers ──────────────────────────────────────────────────────

    void ShowPopup(string msg, Color color)
    {
        if (popupText == null) return;
        StopCoroutine("PopupRoutine");
        StartCoroutine(PopupRoutine(msg, color));
    }

    IEnumerator PopupRoutine(string msg, Color color)
    {
        popupText.text  = msg;
        popupText.color = color;
        popupText.gameObject.SetActive(true);

        float duration = 0.9f;
        float elapsed  = 0f;
        Vector3 basePos = popupText.rectTransform.anchoredPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t   = elapsed / duration;
            float yOff = Mathf.Lerp(0f, 60f, t);
            popupText.rectTransform.anchoredPosition = basePos + new Vector3(0, yOff, 0);

            // Fade out in last 40%
            float alpha = t < 0.6f ? 1f : Mathf.Lerp(1f, 0f, (t - 0.6f) / 0.4f);
            // Scale pop
            float scale = t < 0.1f ? Mathf.Lerp(0.5f, 1.2f, t / 0.1f)
                        : t < 0.2f ? Mathf.Lerp(1.2f, 1.0f, (t - 0.1f) / 0.1f)
                        : 1f;

            popupText.color = new Color(color.r, color.g, color.b, alpha);
            popupText.transform.localScale = Vector3.one * scale;
            yield return null;
        }

        popupText.rectTransform.anchoredPosition = basePos;
        popupText.gameObject.SetActive(false);
    }

    IEnumerator PunchScale(Transform t, float peak, float duration)
    {
        Vector3 orig  = t.localScale;
        Vector3 big   = orig * peak;
        float   half  = duration * 0.5f;
        float   elapsed = 0f;

        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            t.localScale = Vector3.Lerp(orig, big, elapsed / half);
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            t.localScale = Vector3.Lerp(big, orig, elapsed / half);
            yield return null;
        }
        t.localScale = orig;
    }

    void EnsurePopupText()
    {
        if (popupText != null) return;

        // Auto-create a popup TMP text on the canvas
        var canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        var go = new GameObject("PopupText");
        go.transform.SetParent(canvas.transform, false);

        popupText = go.AddComponent<TextMeshProUGUI>();
        popupText.fontSize  = 52;
        popupText.fontStyle = FontStyles.Bold;
        popupText.alignment = TextAlignmentOptions.Center;
        popupText.text      = "";

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0.5f, 0.5f);
        rt.anchorMax        = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0, 80f);
        rt.sizeDelta        = new Vector2(400f, 80f);

        go.SetActive(false);
    }
}
