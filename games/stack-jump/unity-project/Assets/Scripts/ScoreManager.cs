using UnityEngine;
using System;

/// <summary>
/// Tracks current score and all-time best score.
/// Best score is persisted via PlayerPrefs.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int CurrentScore { get; private set; }
    public int BestScore    { get; private set; }

    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnBestChanged;

    private const string BEST_KEY = "StackJump_BestScore";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        BestScore = PlayerPrefs.GetInt(BEST_KEY, 0);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <param name="points">1 = normal, 2 = perfect placement</param>
    public void AddScore(int points = 1)
    {
        CurrentScore += points;

        if (CurrentScore > BestScore)
        {
            BestScore = CurrentScore;
            PlayerPrefs.SetInt(BEST_KEY, BestScore);
            PlayerPrefs.Save();
            OnBestChanged?.Invoke(BestScore);
        }

        OnScoreChanged?.Invoke(CurrentScore);
    }

    public void ResetSession()
    {
        CurrentScore = 0;
    }
}
