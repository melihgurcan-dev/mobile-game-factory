/// <summary>
/// Auto-setup script for Stack Jump.
/// Usage: Unity menu → Stack Jump → Setup Scene
/// This creates the entire scene hierarchy, wires all references, and saves.
/// </summary>
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public static class StackJumpSceneSetup
{
    [MenuItem("Stack Jump/Setup Scene")]
    public static void SetupScene()
    {
        // ── 0. Clear current scene ────────────────────────────────────────
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ── 1. Camera ─────────────────────────────────────────────────────
        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic     = true;
        cam.orthographicSize = 7f;
        cam.backgroundColor  = new Color(0.102f, 0.102f, 0.173f, 1f); // #1A1A2E
        cam.clearFlags       = CameraClearFlags.SolidColor;
        camGO.transform.position = new Vector3(0, 5, -10);
        camGO.AddComponent<AudioListener>();

        // ── 2. _GameManager ───────────────────────────────────────────────
        var mgr = new GameObject("_GameManager");
        var gameManager      = mgr.AddComponent<GameManager>();
        var stackController  = mgr.AddComponent<StackController>();
        var scoreManager     = mgr.AddComponent<ScoreManager>();
        var audioManager     = mgr.AddComponent<AudioManager>();
        var juiceEffects     = mgr.AddComponent<JuiceEffects>();

        stackController.mainCamera  = cam;
        juiceEffects.targetCamera   = cam;

        // ── 3. Canvas ─────────────────────────────────────────────────────
        var canvasGO = new GameObject("Canvas");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight  = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // EventSystem
        var es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // ── Helper: create TMP text ───────────────────────────────────────
        TextMeshProUGUI MakeTMPText(string name, Transform parent, string text, int fontSize, TextAlignmentOptions align, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go  = new GameObject(name);
            go.transform.SetParent(parent, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text      = text;
            tmp.fontSize  = fontSize;
            tmp.alignment = align;
            tmp.color     = Color.white;
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin   = anchorMin;
            rt.anchorMax   = anchorMax;
            rt.pivot       = pivot;
            rt.offsetMin   = offsetMin;
            rt.offsetMax   = offsetMax;
            return tmp;
        }

        // Helper: full-stretch rect
        void SetFullStretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        // Helper: create panel (Image)
        GameObject MakePanel(string name, Transform parent, Color color)
        {
            var go   = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img  = go.AddComponent<Image>();
            img.color = color;
            SetFullStretch(go.GetComponent<RectTransform>());
            return go;
        }

        // Helper: create button
        Button MakeButton(string name, Transform parent, string label, Vector2 size, Vector2 anchoredPos)
        {
            var go  = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = new Color(0.2f, 0.6f, 1f, 1f);
            var btn = go.AddComponent<Button>();
            var rt  = go.GetComponent<RectTransform>();
            rt.sizeDelta     = size;
            rt.anchoredPosition = anchoredPos;

            var lblGO = new GameObject("Label");
            lblGO.transform.SetParent(go.transform, false);
            var lbl = lblGO.AddComponent<TextMeshProUGUI>();
            lbl.text      = label;
            lbl.fontSize  = 48;
            lbl.alignment = TextAlignmentOptions.Center;
            lbl.color     = Color.white;
            SetFullStretch(lblGO.GetComponent<RectTransform>());
            return btn;
        }

        // ── 4. HUD — Score & Best (always visible during play) ────────────
        var ct = canvasGO.transform;

        var scoreTMP = MakeTMPText("ScoreText", ct,
            "0", 96, TextAlignmentOptions.Center,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(-200, -220), new Vector2(200, -100));
        scoreTMP.fontStyle = FontStyles.Bold;
        scoreTMP.gameObject.SetActive(false);

        var bestTMP = MakeTMPText("BestText", ct,
            "Best: 0", 36, TextAlignmentOptions.Center,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(-200, -290), new Vector2(200, -230));
        bestTMP.gameObject.SetActive(false);

        // ── 5. MenuPanel ──────────────────────────────────────────────────
        var menuPanelGO = MakePanel("MenuPanel", ct, new Color(0.05f, 0.05f, 0.12f, 0.95f));

        MakeTMPText("TitleText", menuPanelGO.transform,
            "STACK\nJUMP", 96, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-300, 80), new Vector2(300, 360));

        var menuBestTMP = MakeTMPText("MenuBestText", menuPanelGO.transform,
            "", 40, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-250, -20), new Vector2(250, 60));
        menuBestTMP.color = new Color(0.8f, 0.8f, 0.8f);

        var playBtn = MakeButton("PlayButton", menuPanelGO.transform,
            "PLAY", new Vector2(400, 120), new Vector2(0, -160));

        // ── 6. GameOverPanel ──────────────────────────────────────────────
        var gameOverPanelGO = MakePanel("GameOverPanel", ct, new Color(0.02f, 0.02f, 0.08f, 0.97f));
        gameOverPanelGO.SetActive(false);

        MakeTMPText("GameOverLabel", gameOverPanelGO.transform,
            "GAME OVER", 64, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-300, 200), new Vector2(300, 320));

        MakeTMPText("ScoreLabel", gameOverPanelGO.transform,
            "SCORE", 36, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-200, 80), new Vector2(200, 150));

        var goScoreTMP = MakeTMPText("GameOverScoreText", gameOverPanelGO.transform,
            "0", 120, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-250, -80), new Vector2(250, 100));
        goScoreTMP.fontStyle = FontStyles.Bold;

        var goBestTMP = MakeTMPText("GameOverBestText", gameOverPanelGO.transform,
            "Best: 0", 40, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-250, -160), new Vector2(250, -100));
        goBestTMP.color = new Color(0.8f, 0.8f, 0.8f);

        var newBestTMP = MakeTMPText("NewBestText", gameOverPanelGO.transform,
            "NEW BEST!", 52, TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-250, -220), new Vector2(250, -160));
        newBestTMP.color  = new Color(1f, 0.9f, 0.1f);
        newBestTMP.fontStyle = FontStyles.Bold;
        newBestTMP.gameObject.SetActive(false);

        var retryBtn = MakeButton("RetryButton", gameOverPanelGO.transform,
            "RETRY", new Vector2(400, 120), new Vector2(0, -360));

        // ── 7. Wire UIManager ─────────────────────────────────────────────
        var uiManager = canvasGO.AddComponent<UIManager>();
        uiManager.scoreText            = scoreTMP;
        uiManager.bestText             = bestTMP;
        uiManager.menuPanel            = menuPanelGO;
        uiManager.menuBestScoreText    = menuBestTMP;
        uiManager.gameOverPanel        = gameOverPanelGO;
        uiManager.gameOverScoreText    = goScoreTMP;
        uiManager.gameOverBestText     = goBestTMP;
        uiManager.gameOverNewBestText  = newBestTMP;

        // ── 8. Wire button OnClick events ─────────────────────────────────
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            playBtn.onClick,
            uiManager.OnPlayButton);

        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            retryBtn.onClick,
            uiManager.OnRetryButton);

        // ── 9. Save scene ─────────────────────────────────────────────────
        System.IO.Directory.CreateDirectory(Application.dataPath + "/Scenes");
        string scenePath = "Assets/Scenes/Game.unity";
        EditorSceneManager.SaveScene(scene, scenePath);

        Debug.Log("[StackJump] Scene setup complete! Saved to " + scenePath);
        EditorUtility.DisplayDialog("Stack Jump", "Scene setup complete!\n\nPress Play to test.", "OK");
    }
}
#endif
