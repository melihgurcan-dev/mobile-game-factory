using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Core game logic: manages the block stack, spawning, cutting, and camera.
/// This is the heart of Stack Jump.
/// </summary>
public class StackController : MonoBehaviour
{
    public static StackController Instance { get; private set; }

    [Header("Block Settings")]
    [Tooltip("Width of the very first (base) block")]
    public float initialBlockWidth = 3.0f;
    [Tooltip("Height of every block")]
    public float blockHeight = 0.25f;
    [Tooltip("How far left/right the moving block travels")]
    public float moveRange = 3.5f;

    [Header("Difficulty")]
    public float initialSpeed = 2.5f;
    public float speedIncreasePerBlock = 0.04f;
    public float maxSpeed = 7f;

    [Header("References")]
    public Camera mainCamera;

    // ── Internal state ──────────────────────────────────────────────────────
    private readonly List<GameObject> stack = new List<GameObject>();
    private GameObject movingBlock;
    private float currentBlockWidth;
    private float currentStackTopY;    // Y center of the top stack block
    private float currentSpeed;
    private int nextMoveDirection = 1;  // alternates each block
    private bool gameRunning = false;
    private int blockCount = 0;
    private int comboStreak = 0;       // consecutive perfect placements

    // Fired when a block is placed: (isPerfect, comboStreak)
    public static System.Action<bool, int> OnBlockPlaced;

    // camera
    private float cameraTargetY;
    private const float CAM_Y_OFFSET = 5f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        currentBlockWidth = initialBlockWidth;
        currentSpeed = initialSpeed;
        currentStackTopY = 0f;

        // Spawn static base block
        var baseBlock = InstantiateBlock(0f, 0f, initialBlockWidth, 0);
        stack.Add(baseBlock);

        // Position camera
        if (mainCamera == null) mainCamera = Camera.main;
        cameraTargetY = CAM_Y_OFFSET;
        SetCameraY(cameraTargetY);
    }

    void Update()
    {
        if (!gameRunning || mainCamera == null) return;

        // Smooth camera follow
        var camPos = mainCamera.transform.position;
        camPos.y = Mathf.Lerp(camPos.y, cameraTargetY, Time.deltaTime * 5f);
        mainCamera.transform.position = camPos;
    }

    /// <summary>Called by GameManager.StartGame()</summary>
    public void BeginGame()
    {
        currentBlockWidth = initialBlockWidth;  // always reset on (re)start
        currentSpeed      = initialSpeed;
        comboStreak       = 0;
        gameRunning = true;
        SpawnMovingBlock();
    }

    /// <summary>Called on player tap/click by GameManager.</summary>
    public void PlaceBlock()
    {
        if (movingBlock == null || !gameRunning) return;

        // Stop the horizontal movement
        var mb = movingBlock.GetComponent<MovingBlock>();
        mb.Stop();

        float movingX = movingBlock.transform.position.x;
        float movingW = movingBlock.transform.localScale.x;

        var topBlock = stack[stack.Count - 1];
        float stackX = topBlock.transform.position.x;
        float stackW = topBlock.transform.localScale.x;

        // ── Overlap calculation ────────────────────────────────────────
        float movingLeft  = movingX - movingW * 0.5f;
        float movingRight = movingX + movingW * 0.5f;
        float stackLeft   = stackX  - stackW  * 0.5f;
        float stackRight  = stackX  + stackW  * 0.5f;

        float overlapLeft  = Mathf.Max(movingLeft,  stackLeft);
        float overlapRight = Mathf.Min(movingRight, stackRight);
        float overlapWidth = overlapRight - overlapLeft;

        if (overlapWidth <= 0.02f)
        {
            // Complete miss — game over
            DropPiece(movingX, movingBlock.transform.position.y, movingW);
            Destroy(movingBlock);
            movingBlock = null;
            AudioManager.Instance?.PlayFail();
            GameManager.Instance.TriggerGameOver();
            return;
        }

        // ── Perfect placement ──────────────────────────────────────────
        bool isPerfect = Mathf.Abs(movingX - stackX) < 0.12f;
        float finalWidth = isPerfect ? stackW : overlapWidth;

        if (isPerfect)
        {
            comboStreak++;
            ResizeAndReposition(movingBlock, stackX, finalWidth, currentStackTopY + blockHeight);
            if (comboStreak >= 3)
                AudioManager.Instance?.PlayCombo();
            else
                AudioManager.Instance?.PlayPerfect();
        }
        else
        {
            comboStreak = 0;
            float overlapCenterX = (overlapLeft + overlapRight) * 0.5f;

            float hangWidth = movingW - overlapWidth;
            float hangCenterX = movingX > stackX
                ? overlapRight + hangWidth * 0.5f
                : overlapLeft  - hangWidth * 0.5f;

            DropPiece(hangCenterX, movingBlock.transform.position.y, hangWidth);
            ResizeAndReposition(movingBlock, overlapCenterX, overlapWidth, currentStackTopY + blockHeight);
            currentBlockWidth = overlapWidth;
            AudioManager.Instance?.PlayPlace();
        }

        // Squish animation on placed block
        JuiceEffects.Instance?.SquishBlock(movingBlock);

        stack.Add(movingBlock);
        movingBlock = null;
        blockCount++;
        currentStackTopY += blockHeight;

        // Combo score: +1 base, +1 per streak level (capped at +4 bonus)
        int bonus = isPerfect ? Mathf.Min(comboStreak, 5) : 1;
        ScoreManager.Instance?.AddScore(bonus);
        OnBlockPlaced?.Invoke(isPerfect, comboStreak);
        cameraTargetY = currentStackTopY + CAM_Y_OFFSET;

        // Speed increase
        currentSpeed = Mathf.Min(currentSpeed + speedIncreasePerBlock, maxSpeed);

        SpawnMovingBlock();
    }

    // ── Private helpers ────────────────────────────────────────────────────

    void SpawnMovingBlock()
    {
        float spawnY = currentStackTopY + blockHeight;
        float startX = moveRange * nextMoveDirection;
        nextMoveDirection *= -1;

        movingBlock = InstantiateBlock(startX, spawnY, currentBlockWidth, blockCount + 1);
        movingBlock.AddComponent<MovingBlock>().Initialize(currentSpeed, moveRange, -nextMoveDirection);
    }

    void ResizeAndReposition(GameObject block, float x, float width, float y)
    {
        block.transform.position   = new Vector3(x, y, 0f);
        block.transform.localScale = new Vector3(width, blockHeight, 1f);
    }

    void DropPiece(float xCenter, float y, float width)
    {
        var piece = InstantiateBlock(xCenter, y, width, -1);
        float nudge = xCenter > 0 ? 1.5f : -1.5f;
        piece.AddComponent<FallingPiece>().Launch(nudge);
        Destroy(piece, 2f);
    }

    GameObject InstantiateBlock(float x, float y, float width, int colorIndex)
    {
        // Quad has no collider — avoids Rigidbody2D conflict entirely
        var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = "Block_" + blockCount;
        go.transform.position   = new Vector3(x, y, 0f);
        go.transform.localScale = new Vector3(width, blockHeight, 1f);

        // Color — use Unlit/Color so shader is always included in WebGL/Android builds
        var sr = go.GetComponent<Renderer>();
        var mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = colorIndex < 0
            ? new Color(0.4f, 0.4f, 0.4f, 0.8f)
            : GameColors.GetColor(colorIndex);
        sr.material = mat;

        return go;
    }

    void SetCameraY(float y)
    {
        if (mainCamera == null) return;
        var pos = mainCamera.transform.position;
        pos.y = y;
        mainCamera.transform.position = pos;
    }
}
