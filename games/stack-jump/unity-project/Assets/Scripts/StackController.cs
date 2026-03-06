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

    // camera
    private float cameraTargetY;
    private const float CAM_Y_OFFSET = 5f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
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
            DropPiece(movingBlock, movingX, movingW);
            movingBlock = null;
            AudioManager.Instance?.PlayFail();
            GameManager.Instance.TriggerGameOver();
            return;
        }

        // ── Perfect placement bonus ────────────────────────────────────
        bool isPerfect = Mathf.Abs(movingX - stackX) < 0.08f;
        float finalWidth = isPerfect ? stackW : overlapWidth;

        if (isPerfect)
        {
            // Align perfectly, no cut
            ResizeAndReposition(movingBlock, stackX, finalWidth, currentStackTopY + blockHeight);
            AudioManager.Instance?.PlayPerfect();
        }
        else
        {
            float overlapCenterX = (overlapLeft + overlapRight) * 0.5f;

            // Spawn the falling off-cut piece
            float hangWidth = movingW - overlapWidth;
            float hangCenterX = movingX > stackX
                ? overlapRight + hangWidth * 0.5f
                : overlapLeft  - hangWidth * 0.5f;

            DropPiece(movingBlock, hangCenterX, hangWidth);

            // Resize the placed block to the overlapping portion
            ResizeAndReposition(movingBlock, overlapCenterX, overlapWidth, currentStackTopY + blockHeight);
            currentBlockWidth = overlapWidth;
            AudioManager.Instance?.PlayPlace();
        }

        stack.Add(movingBlock);
        movingBlock = null;
        blockCount++;
        currentStackTopY += blockHeight;

        ScoreManager.Instance?.AddScore(isPerfect ? 2 : 1);
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

    void DropPiece(GameObject sourceBlock, float xCenter, float width)
    {
        // Create a separate falling block for the cut-off piece
        var piece = InstantiateBlock(xCenter, sourceBlock.transform.position.y, width, -1);
        var rb = piece.AddComponent<Rigidbody2D>();
        rb.gravityScale = 2f;
        float nudge = xCenter > 0 ? 1.2f : -1.2f;
        rb.AddForce(new Vector2(nudge, -0.3f), ForceMode2D.Impulse);
        Destroy(piece, 2f);
        Destroy(sourceBlock);
    }

    GameObject InstantiateBlock(float x, float y, float width, int colorIndex)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Block_" + blockCount;
        go.transform.position   = new Vector3(x, y, 0f);
        go.transform.localScale = new Vector3(width, blockHeight, 1f);

        // Remove the 3D collider — we don't need physics for stacked blocks
        Destroy(go.GetComponent<BoxCollider>());

        // Color
        var sr = go.GetComponent<Renderer>();
        sr.material = new Material(Shader.Find("Sprites/Default"));
        sr.material.color = colorIndex < 0
            ? new Color(0.4f, 0.4f, 0.4f, 0.8f)   // cut-off piece: grey semitransparent
            : GameColors.GetColor(colorIndex);

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
