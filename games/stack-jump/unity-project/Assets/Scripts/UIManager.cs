using UnityEngine;
using TMPro;

/// <summary>
/// Manages all UI: Menu panel, HUD (score), GameOver panel.
/// Wire up references in the Inspector, then assign button OnClick events to
/// OnPlayButton() and OnRetryButton().
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
    public TextMeshProUGUI gameOverNewBestText;   // "NEW BEST!" label

    // ── Lifecycle ──────────────────────────────────────────────────────────

    void OnEnable()
    {
        GameManager.OnGameStart   += HandleGameStart;
        GameManager.OnGameOver    += HandleGameOver;
        ScoreManager.OnScoreChanged += UpdateHUDScore;
        ScoreManager.OnBestChanged  += HandleNewBest;
    }

    void OnDisable()
    {
        GameManager.OnGameStart   -= HandleGameStart;
        GameManager.OnGameOver    -= HandleGameOver;
        ScoreManager.OnScoreChanged -= UpdateHUDScore;
        ScoreManager.OnBestChanged  -= HandleNewBest;
    }

    void Start()
    {
        // Show menu on start; hide everything else
        menuPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        scoreText.gameObject.SetActive(false);
        bestText.gameObject.SetActive(false);

        if (gameOverNewBestText) gameOverNewBestText.gameObject.SetActive(false);

        int best = ScoreManager.Instance ? ScoreManager.Instance.BestScore : 0;
        if (menuBestScoreText) menuBestScoreText.text = best > 0 ? $"Best: {best}" : "";
    }

    // ── Event handlers ─────────────────────────────────────────────────────

    void HandleGameStart()
    {
        menuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        scoreText.gameObject.SetActive(true);
        bestText.gameObject.SetActive(true);
        scoreText.text = "0";
        bestText.text  = $"Best: {ScoreManager.Instance.BestScore}";
        if (gameOverNewBestText) gameOverNewBestText.gameObject.SetActive(false);
    }

    void HandleGameOver()
    {
        gameOverPanel.SetActive(true);
        int score = ScoreManager.Instance.CurrentScore;
        int best  = ScoreManager.Instance.BestScore;
        gameOverScoreText.text = score.ToString();
        gameOverBestText.text  = $"Best: {best}";
    }

    void UpdateHUDScore(int score)
    {
        scoreText.text = score.ToString();
    }

    void HandleNewBest(int best)
    {
        bestText.text = $"Best: {best}";
        if (gameOverNewBestText) gameOverNewBestText.gameObject.SetActive(true);
    }

    // ── Button callbacks (wire in Inspector) ──────────────────────────────

    public void OnPlayButton()  => GameManager.Instance.StartGame();
    public void OnRetryButton() => GameManager.Instance.RestartGame();
}
