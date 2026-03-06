using UnityEngine;
using System;

/// <summary>
/// Central game state machine. Handles input and coordinates all systems.
/// Attach to a persistent GameObject in the Game scene.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Menu, Playing, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Menu;

    // Events — subscribe in UIManager, AudioManager, etc.
    public static event Action OnGameStart;
    public static event Action OnGameOver;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (CurrentState != GameState.Playing) return;

        // Tap / click input
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            StackController.Instance?.PlaceBlock();
        }
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        OnGameStart?.Invoke();
        StackController.Instance?.BeginGame();
    }

    public void TriggerGameOver()
    {
        CurrentState = GameState.GameOver;
        OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.name);
    }
}
