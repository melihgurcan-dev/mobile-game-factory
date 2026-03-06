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
        if (menuPanel)        menuPanel.SetActive(true);
        if (gameOverPanel)    gameOverPanel.SetActive(false);
        if (scoreText)        scoreText.gameObject.SetActive(false);
        if (bestText)         bestText.gameObject.SetActive(false);
        if (gameOverNewBestText) gameOverNewBestText.gameObject.SetActive(false);

        int best = ScoreManager.Instance ? ScoreManager.Instance.BestScore : 0;
        if (menuBestScoreText) menuBestScoreText.text = best > 0 ? $"Best: {best}" : "";

        // If menu panel isn't wired, auto-start immediately
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
    }

    void UpdateHUDScore(int score)
    {
        if (scoreText) scoreText.text = score.ToString();
    }

    void HandleNewBest(int best)
    {
        if (bestText) bestText.text = $"Best: {best}";
        if (gameOverNewBestText) gameOverNewBestText.gameObject.SetActive(true);
    }

    // ── Button callbacks (wire in Inspector) ──────────────────────────────

    public void OnPlayButton()  => GameManager.Instance.StartGame();
    public void OnRetryButton() => GameManager.Instance.RestartGame();
}
