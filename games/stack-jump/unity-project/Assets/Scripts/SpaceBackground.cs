using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 6-stage space journey background. Each 10 blocks = new chapter.
/// Auto-injects at runtime â€” no scene setup needed.
/// </summary>
public class SpaceBackground : MonoBehaviour
{
    // â”€â”€ Journey stages â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    struct Stage
    {
        public Color  Sky;
        public Color  StarTint;
        public Color  NebulaCol;
        public Color  PlanetCol;
        public float  PlanetSize;
        public string Title;
        public string Subtitle;
    }

    static readonly Stage[] Stages = new Stage[]
    {
        new Stage {
            Sky        = new Color(0.03f, 0.04f, 0.12f),
            StarTint   = new Color(0.80f, 0.88f, 1.00f),
            NebulaCol  = new Color(0.10f, 0.20f, 0.60f, 0.07f),
            PlanetCol  = new Color(0.20f, 0.50f, 0.90f),   // blue Earth
            PlanetSize = 1.6f,
            Title      = "DÃœNYA YÃ–RÃœNGESÄ°",
            Subtitle   = "Roketin fÄ±rladÄ±! Uzay seni bekliyor...",
        },
        new Stage {
            Sky        = new Color(0.01f, 0.01f, 0.09f),
            StarTint   = new Color(0.70f, 0.80f, 1.00f),
            NebulaCol  = new Color(0.15f, 0.25f, 0.70f, 0.09f),
            PlanetCol  = new Color(0.70f, 0.45f, 0.20f),   // Mars-ish
            PlanetSize = 1.2f,
            Title      = "DERÄ°N UZAY",
            Subtitle   = "YÄ±ldÄ±zlar arasÄ±nda sessizlik...",
        },
        new Stage {
            Sky        = new Color(0.06f, 0.01f, 0.15f),
            StarTint   = new Color(0.80f, 0.55f, 1.00f),
            NebulaCol  = new Color(0.50f, 0.10f, 0.80f, 0.12f),
            PlanetCol  = new Color(0.55f, 0.20f, 0.85f),   // purple gas giant
            PlanetSize = 2.2f,
            Title      = "NEBULA BULUTU",
            Subtitle   = "Mor sis seni sarÄ±yor...",
        },
        new Stage {
            Sky        = new Color(0.12f, 0.03f, 0.04f),
            StarTint   = new Color(1.00f, 0.52f, 0.36f),
            NebulaCol  = new Color(0.80f, 0.18f, 0.10f, 0.11f),
            PlanetCol  = new Color(0.90f, 0.30f, 0.10f),   // red giant
            PlanetSize = 3.0f,
            Title      = "KIRMIZI DEV",
            Subtitle   = "Dev yÄ±ldÄ±z seni Ä±sÄ±tÄ±yor!",
        },
        new Stage {
            Sky        = new Color(0.14f, 0.07f, 0.01f),
            StarTint   = new Color(1.00f, 0.88f, 0.40f),
            NebulaCol  = new Color(0.90f, 0.55f, 0.10f, 0.13f),
            PlanetCol  = new Color(1.00f, 0.75f, 0.20f),   // golden
            PlanetSize = 2.5f,
            Title      = "SÃœPERNOVA ARTIKLARI",
            Subtitle   = "AltÄ±n toz seni kapladÄ±!",
        },
        new Stage {
            Sky        = new Color(0.16f, 0.02f, 0.10f),
            StarTint   = new Color(1.00f, 1.00f, 1.00f),
            NebulaCol  = new Color(0.90f, 0.20f, 0.60f, 0.15f),
            PlanetCol  = new Color(1.00f, 0.90f, 0.98f),   // white/pink galactic core
            PlanetSize = 3.8f,
            Title      = "GALAKSÄ° MERKEZÄ°",
            Subtitle   = "Efsanevi Ã¶tesi â€” sen bir kahraman!",
        },
    };

    // â”€â”€ Config â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    const int   STAR_COUNT     = 130;
    const int   NEBULA_COUNT   = 7;
    const float TRANSITION     = 3.0f;
    const float SHOOT_INTERVAL = 4.5f;
    const float STAR_DEPTH     = 8f;
    const float NEBULA_DEPTH   = 9f;
    const float PLANET_DEPTH   = 7.5f;

    // â”€â”€ State â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    Camera            _cam;
    SpriteRenderer[]  _stars;
    float[]           _twinkleOffset;
    float[]           _twinkleSpeed;
    SpriteRenderer[]  _nebulae;
    GameObject        _planet;
    SpriteRenderer    _planetSR;
    int               _stage = -1;
    float             _nextShoot;
    Sprite            _dotSprite;
    Sprite            _softSprite;

    // Journey label (world-space canvas)
    GameObject  _labelRoot;
    TextMeshPro _titleTMP;
    TextMeshPro _subTMP;

    // Astronaut companion
    GameObject   _astro;
    SpriteRenderer _astroSR;
    TextMeshPro  _bubbleTMP;
    int          _lastMilestone = -1;
    float        _astroBobOffset;
    const float  ASTRO_DEPTH = 6.5f;

    static readonly string[] AstroMessages = {
        "HARIKA!\nDevam et!",
        "WOW!\nInanilmaz!",
        "SUPER!\nHadi hadi!",
        "BRAVO!\nVay be!",
        "EFSANE!\nBos durma!",
        "MAVSALL!\nCok guzel!",
        "AMAZING!\nUspacisin!",
        "YAY!\nKahramanim!",
    };

    // â”€â”€ Auto-inject â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<SpaceBackground>() == null)
        {
            var go = new GameObject("SpaceBackground");
            DontDestroyOnLoad(go);
            go.AddComponent<SpaceBackground>();
        }
    }

    // â”€â”€ Lifecycle â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    void Start()
    {
        _cam        = Camera.main;
        _dotSprite  = MakeGlowSprite(16, 0.9f);
        _softSprite = MakeGlowSprite(64, 0.45f);

        SpawnNebulae();
        SpawnPlanet();
        SpawnStars();
        SpawnJourneyLabel();
        SpawnAstronaut();
        ApplyStage(0, instant: true);

        _nextShoot = Time.time + Random.Range(2f, SHOOT_INTERVAL);
    }

    void OnEnable()  => StackController.OnBlockPlaced += OnBlock;
    void OnDisable() => StackController.OnBlockPlaced -= OnBlock;

    void OnBlock(bool isPerfect, int combo)
    {
        int score = ScoreManager.Instance != null ? ScoreManager.Instance.CurrentScore : 0;
        int s = Mathf.Min(score / 10, Stages.Length - 1);
        if (s != _stage) ApplyStage(s, instant: false);

        int milestone = score / 15;
        if (milestone > _lastMilestone)
        {
            _lastMilestone = milestone;
            StartCoroutine(ShowAstroMessage(AstroMessages[milestone % AstroMessages.Length]));
        }
    }

    void Update()
    {
        TwinkleStars();
        RotatePlanet();
        BobAstronaut();

        // Sync label position (stays just above blocks)
        if (_labelRoot != null && _cam != null)
        {
            float hs = _cam.orthographicSize;
            _labelRoot.transform.position = new Vector3(-3.5f, hs * 0.72f, 6f);
        }

        if (Time.time >= _nextShoot)
        {
            StartCoroutine(ShootingStar());
            _nextShoot = Time.time + Random.Range(SHOOT_INTERVAL * 0.5f, SHOOT_INTERVAL * 1.5f);
        }
    }

    // â”€â”€ Stage transition â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    void ApplyStage(int s, bool instant)
    {
        _stage = s;
        var st = Stages[s];
        StopAllCoroutines();

        if (instant)
        {
            SetCamColor(st.Sky);
            TintStars(st.StarTint, 1f);
            TintNebulae(st.NebulaCol, 1f);
            SetPlanet(st.PlanetCol, st.PlanetSize, 1f);
            SetLabel(st.Title, st.Subtitle, 0f);
        }
        else
        {
            StartCoroutine(TransitionCam(s));
            StartCoroutine(TransitionStars(s));
            StartCoroutine(TransitionNebulae(s));
            StartCoroutine(TransitionPlanet(s));
            StartCoroutine(ShowLabel(st.Title, st.Subtitle));
            StartCoroutine(ShootingStar()); // celebrate
        }
    }

    IEnumerator TransitionCam(int s)
    {
        Color from = _cam ? _cam.backgroundColor : Stages[s].Sky;
        for (float t = 0; t < 1f; t += Time.deltaTime / TRANSITION)
        { SetCamColor(Color.Lerp(from, Stages[s].Sky, Smooth(t))); yield return null; }
        SetCamColor(Stages[s].Sky);
    }

    IEnumerator TransitionStars(int s)
    {
        Color to = Stages[s].StarTint;
        for (float t = 0; t < 1f; t += Time.deltaTime / TRANSITION)
        { TintStars(to, Smooth(t)); yield return null; }
        TintStars(to, 1f);
    }

    IEnumerator TransitionNebulae(int s)
    {
        Color to = Stages[s].NebulaCol;
        for (float t = 0; t < 1f; t += Time.deltaTime / TRANSITION)
        { TintNebulae(to, Smooth(t)); yield return null; }
        TintNebulae(to, 1f);
    }

    IEnumerator TransitionPlanet(int s)
    {
        if (_planet == null) yield break;
        Color fromCol  = _planetSR.color;
        float fromSize = _planet.transform.localScale.x;
        float toSize   = Stages[s].PlanetSize;
        Color toCol    = Stages[s].PlanetCol;
        // Pulse UP then settle at new size
        for (float t = 0; t < 1f; t += Time.deltaTime / TRANSITION)
        {
            float pulse = 1f + 0.20f * Mathf.Sin(Mathf.PI * t);
            float sz    = Mathf.Lerp(fromSize, toSize, Smooth(t)) * pulse;
            _planet.transform.localScale = Vector3.one * sz;
            _planetSR.color = Color.Lerp(fromCol, toCol, Smooth(t));
            yield return null;
        }
        SetPlanet(toCol, toSize, 1f);
    }

    // â”€â”€ Journey label â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    void SpawnJourneyLabel()
    {
        _labelRoot = new GameObject("JourneyLabel");
        _labelRoot.transform.SetParent(transform);

        _titleTMP = MakeTMP("Title", _labelRoot.transform, 0.55f, FontStyles.Bold);
        _titleTMP.transform.localPosition = Vector3.zero;

        _subTMP = MakeTMP("Sub", _labelRoot.transform, 0.28f, FontStyles.Normal);
        _subTMP.transform.localPosition = new Vector3(0, -0.7f, 0);

        // Start transparent
        SetLabel("", "", 0f);
    }

    TextMeshPro MakeTMP(string name, Transform parent, float size, FontStyles style)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshPro>();
        tmp.fontSize    = size;
        tmp.fontStyle   = style;
        tmp.color       = new Color(1, 1, 1, 0);
        tmp.alignment   = TextAlignmentOptions.Left;
        tmp.sortingOrder = -6;
        return tmp;
    }

    void SetLabel(string title, string sub, float alpha)
    {
        if (_titleTMP == null) return;
        _titleTMP.text = title;
        _subTMP.text   = sub;
        var tc = _titleTMP.color; tc.a = alpha; _titleTMP.color = tc;
        var sc = _subTMP.color;   sc.a = alpha; _subTMP.color   = sc;
    }

    IEnumerator ShowLabel(string title, string sub)
    {
        _titleTMP.text = title;
        _subTMP.text   = sub;

        // Fade in
        for (float t = 0; t < 1f; t += Time.deltaTime / 0.6f)
        { SetLabelAlpha(Smooth(t)); yield return null; }
        SetLabelAlpha(1f);

        yield return new WaitForSeconds(2.8f);

        // Fade out
        for (float t = 0; t < 1f; t += Time.deltaTime / 0.8f)
        { SetLabelAlpha(1f - Smooth(t)); yield return null; }
        SetLabelAlpha(0f);
    }

    void SetLabelAlpha(float a)
    {
        var tc = _titleTMP.color; tc.a = a; _titleTMP.color = tc;
        var sc = _subTMP.color;   sc.a = a; _subTMP.color   = sc;
    }

    // â”€â”€ Shooting star â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    IEnumerator ShootingStar()
    {
        float hs = _cam ? _cam.orthographicSize : 7f;
        float hw = _cam ? hs * _cam.aspect      : 4f;

        var go = new GameObject("ShootingStar");
        go.transform.SetParent(transform);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite       = _dotSprite;
        sr.sortingOrder = -5;
        go.transform.localScale = Vector3.one * Random.Range(0.06f, 0.13f);

        float startX = Random.Range(-hw, hw * 0.4f);
        float startY = Random.Range(hs * 0.3f, hs);
        Vector2 vel  = new Vector2(Random.Range(9f, 15f), Random.Range(-11f, -6f));

        for (float t = 0; t < 0.55f; t += Time.deltaTime)
        {
            go.transform.position = new Vector3(startX + vel.x * t, startY + vel.y * t, STAR_DEPTH - 1f);
            float a = Mathf.Sin(Mathf.PI * t / 0.55f);
            sr.color = new Color(1, 1, 1, a);
            yield return null;
        }
        Destroy(go);
    }

    // â”€â”€ Spawn â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    void SpawnNebulae()
    {
        float hs = _cam ? _cam.orthographicSize : 7f;
        float hw = _cam ? hs * _cam.aspect      : 4f;
        _nebulae = new SpriteRenderer[NEBULA_COUNT];
        for (int i = 0; i < NEBULA_COUNT; i++)
        {
            var go = new GameObject($"Nebula_{i}");
            go.transform.SetParent(transform);
            float sz = Random.Range(4f, 8f);
            go.transform.position   = new Vector3(
                Random.Range(-hw * 1.3f, hw * 1.3f),
                Random.Range(-hs, hs * 1.4f), NEBULA_DEPTH);
            go.transform.localScale = Vector3.one * sz;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = _softSprite;
            sr.sortingOrder = -12;
            sr.color        = new Color(0.1f, 0.2f, 0.8f, 0.07f);
            _nebulae[i] = sr;
        }
    }

    void SpawnPlanet()
    {
        float hs = _cam ? _cam.orthographicSize : 7f;
        float hw = _cam ? hs * _cam.aspect      : 4f;
        _planet = new GameObject("Planet");
        _planet.transform.SetParent(transform);
        _planet.transform.position   = new Vector3(hw * 0.60f, hs * 0.62f, PLANET_DEPTH);
        _planet.transform.localScale = Vector3.one * 1.6f;
        _planetSR               = _planet.AddComponent<SpriteRenderer>();
        _planetSR.sprite        = MakePlanetSprite(64);
        _planetSR.sortingOrder  = -8;
    }

    void SpawnStars()
    {
        float hs = _cam ? _cam.orthographicSize : 7f;
        float hw = _cam ? hs * _cam.aspect      : 4f;
        _stars         = new SpriteRenderer[STAR_COUNT];
        _twinkleOffset = new float[STAR_COUNT];
        _twinkleSpeed  = new float[STAR_COUNT];
        for (int i = 0; i < STAR_COUNT; i++)
        {
            var go = new GameObject($"Star_{i}");
            go.transform.SetParent(transform);
            go.transform.position = new Vector3(
                Random.Range(-hw * 1.1f, hw * 1.1f),
                Random.Range(-hs * 1.2f, hs * 1.2f), STAR_DEPTH);
            float sz = Random.value < 0.82f ? Random.Range(0.03f, 0.09f) : Random.Range(0.12f, 0.24f);
            go.transform.localScale = Vector3.one * sz;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = _dotSprite;
            sr.sortingOrder = -10;
            sr.color        = new Color(0.85f, 0.90f, 1f, Random.Range(0.5f, 1f));
            _stars[i]         = sr;
            _twinkleOffset[i] = Random.Range(0f, Mathf.PI * 2f);
            _twinkleSpeed[i]  = Random.Range(0.6f, 2.4f);
        }
    }

    // â”€â”€ Per-frame â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    void TwinkleStars()
    {
        if (_stars == null) return;
        float now = Time.time;
        for (int i = 0; i < _stars.Length; i++)
        {
            if (_stars[i] == null) continue;
            float a = 0.28f + 0.72f * (Mathf.Sin(now * _twinkleSpeed[i] + _twinkleOffset[i]) * 0.5f + 0.5f);
            var c = _stars[i].color; c.a = a; _stars[i].color = c;
        }
    }

    void RotatePlanet()
    {
        if (_planet == null) return;
        _planet.transform.Rotate(0, 0, Time.deltaTime * 2f);
    }

    // â”€â”€ Tint helpers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    void TintStars(Color to, float blend)
    {
        if (_stars == null) return;
        foreach (var sr in _stars)
        {
            if (sr == null) continue;
            var c = sr.color;
            c.r = Mathf.Lerp(c.r, to.r, blend);
            c.g = Mathf.Lerp(c.g, to.g, blend);
            c.b = Mathf.Lerp(c.b, to.b, blend);
            sr.color = c;
        }
    }

    void TintNebulae(Color to, float blend)
    {
        if (_nebulae == null) return;
        foreach (var sr in _nebulae)
        {
            if (sr == null) continue;
            sr.color = Color.Lerp(sr.color, to, blend);
        }
    }

    void SetPlanet(Color col, float size, float blend)
    {
        if (_planet == null) return;
        _planetSR.color = Color.Lerp(_planetSR.color, col, blend);
        float cur = _planet.transform.localScale.x;
        _planet.transform.localScale = Vector3.one * Mathf.Lerp(cur, size, blend);
    }

    void SetCamColor(Color c) { if (_cam) _cam.backgroundColor = c; }
    static float Smooth(float t) => t * t * (3f - 2f * t);

    // ── Astronaut methods ──────────────────────────────────────────────────

    void SpawnAstronaut()
    {
        float hs = _cam ? _cam.orthographicSize : 7f;
        float hw = _cam ? hs * _cam.aspect      : 4f;

        _astro = new GameObject("Astronaut");
        _astro.transform.SetParent(transform);
        _astro.transform.localScale = Vector3.one * 1.4f;
        _astroBobOffset = Random.Range(0f, Mathf.PI * 2f);

        _astroSR              = _astro.AddComponent<SpriteRenderer>();
        _astroSR.sprite       = MakeAstronautSprite(64);
        _astroSR.sortingOrder = -3;

        // Speech bubble text just above astronaut
        var bubbleGO = new GameObject("AstroBubble");
        bubbleGO.transform.SetParent(_astro.transform, false);
        bubbleGO.transform.localPosition = new Vector3(0.8f, 1.5f, 0f);

        _bubbleTMP              = bubbleGO.AddComponent<TextMeshPro>();
        _bubbleTMP.fontSize     = 0.55f;
        _bubbleTMP.fontStyle    = FontStyles.Bold;
        _bubbleTMP.color        = new Color(1f, 1f, 0.4f, 0f);
        _bubbleTMP.alignment    = TextAlignmentOptions.Left;
        _bubbleTMP.sortingOrder = -2;
    }

    void BobAstronaut()
    {
        if (_astro == null || _cam == null) return;
        float hs     = _cam.orthographicSize;
        float hw     = hs * _cam.aspect;
        float camY   = _cam.transform.position.y;
        float camX   = _cam.transform.position.x;
        // Gentle dual-frequency bob in camera space
        float bobY   = Mathf.Sin(Time.time * 0.9f + _astroBobOffset) * 0.55f
                     + Mathf.Sin(Time.time * 0.4f + _astroBobOffset * 0.7f) * 0.25f;
        // Slow left-right drift
        float driftX = Mathf.Sin(Time.time * 0.25f + _astroBobOffset) * 0.35f;
        // Astronaut sits on the left side of the screen, mid-height, following camera
        float targetX = camX - hw * 0.78f + driftX;
        float targetY = camY - hs * 0.15f + bobY;
        _astro.transform.position = new Vector3(targetX, targetY, ASTRO_DEPTH);
    }

    IEnumerator ShowAstroMessage(string msg)
    {
        if (_bubbleTMP == null) yield break;
        _bubbleTMP.text = msg;

        for (float t = 0; t < 1f; t += Time.deltaTime / 0.35f)
        {
            float pulse = 1f + 0.25f * Mathf.Sin(Mathf.PI * t);
            _astro.transform.localScale = Vector3.one * 1.4f * pulse;
            SetBubbleAlpha(Smooth(t));
            yield return null;
        }
        _astro.transform.localScale = Vector3.one * 1.4f;
        SetBubbleAlpha(1f);

        yield return new WaitForSeconds(2.2f);

        for (float t = 0; t < 1f; t += Time.deltaTime / 0.4f)
        { SetBubbleAlpha(1f - Smooth(t)); yield return null; }
        SetBubbleAlpha(0f);
    }

    void SetBubbleAlpha(float a)
    {
        if (_bubbleTMP == null) return;
        var c = _bubbleTMP.color; c.a = a; _bubbleTMP.color = c;
    }

    static Sprite MakeAstronautSprite(int sz)
    {
        var tex = new Texture2D(sz, sz, TextureFormat.RGBA32, false);
        var px  = new Color32[sz * sz];
        for (int i = 0; i < px.Length; i++) px[i] = new Color32(0, 0, 0, 0);

        float cx     = sz * 0.5f;
        float helmCY = sz * 0.68f;
        float helmR  = sz * 0.25f;

        Color32 white  = new Color32(235, 242, 255, 255);
        Color32 visor  = new Color32( 50, 110, 220, 255);
        Color32 suit   = new Color32(255, 150,  35, 255);
        Color32 dark   = new Color32( 40,  50,  70, 255);
        Color32 glove  = new Color32(255, 200,  80, 255);

        for (int y = 0; y < sz; y++)
        for (int x = 0; x < sz; x++)
        {
            float px_x = x + 0.5f, px_y = y + 0.5f;

            float dHelm   = Mathf.Sqrt((px_x-cx)*(px_x-cx) + (px_y-helmCY)*(px_y-helmCY));
            bool inHelmet = dHelm < helmR;

            float vx = (px_x - cx)     / (helmR * 0.58f);
            float vy = (px_y - helmCY) / (helmR * 0.36f);
            bool inVisor = vx*vx + vy*vy < 1f;

            float bodyTop = helmCY - helmR * 0.25f;
            float bodyBot = sz * 0.10f;
            float bodyL   = cx - sz * 0.19f;
            float bodyR_  = cx + sz * 0.19f;
            bool inBody = px_x > bodyL && px_x < bodyR_ && px_y < bodyTop && px_y > bodyBot;

            float armTop = bodyTop - helmR * 0.05f;
            float armBot = bodyTop - helmR * 0.7f;
            bool inLeftArm  = px_x > bodyL  - sz*0.13f && px_x < bodyL  && px_y < armTop && px_y > armBot;
            bool inRightArm = px_x > bodyR_ && px_x < bodyR_ + sz*0.13f && px_y < armTop && px_y > armBot;

            float gloveR  = sz * 0.05f, gloveY = armBot + gloveR;
            float lGloveX = bodyL  - sz*0.065f;
            float rGloveX = bodyR_ + sz*0.065f;
            bool inLG = Mathf.Sqrt((px_x-lGloveX)*(px_x-lGloveX)+(px_y-gloveY)*(px_y-gloveY)) < gloveR;
            bool inRG = Mathf.Sqrt((px_x-rGloveX)*(px_x-rGloveX)+(px_y-gloveY)*(px_y-gloveY)) < gloveR;

            bool helmRing = dHelm >= helmR - 1.5f && dHelm < helmR;

            if      (inVisor)                         px[y*sz+x] = visor;
            else if (helmRing)                         px[y*sz+x] = dark;
            else if (inHelmet)                         px[y*sz+x] = white;
            else if (inLG || inRG)                     px[y*sz+x] = glove;
            else if (inBody || inLeftArm || inRightArm) px[y*sz+x] = suit;
        }

        tex.SetPixels32(px);
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        return Sprite.Create(tex, new Rect(0, 0, sz, sz), new Vector2(0.5f, 0.5f), sz);
    }



    // â”€â”€ Sprite factories â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    static Sprite MakeGlowSprite(int sz, float power)
    {
        var tex    = new Texture2D(sz, sz, TextureFormat.RGBA32, false);
        float half = sz / 2f;
        var px     = new Color32[sz * sz];
        for (int y = 0; y < sz; y++)
        for (int x = 0; x < sz; x++)
        {
            float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(half, half));
            float a = Mathf.Pow(Mathf.Clamp01(1f - d / half), power);
            px[y * sz + x] = new Color32(255, 255, 255, (byte)(a * 255));
        }
        tex.SetPixels32(px); tex.Apply(); tex.filterMode = FilterMode.Bilinear;
        return Sprite.Create(tex, new Rect(0, 0, sz, sz), new Vector2(0.5f, 0.5f), sz);
    }

    static Sprite MakePlanetSprite(int sz)
    {
        var tex  = new Texture2D(sz, sz, TextureFormat.RGBA32, false);
        float half = sz / 2f;
        var px   = new Color32[sz * sz];
        for (int y = 0; y < sz; y++)
        for (int x = 0; x < sz; x++)
        {
            float dx = x + 0.5f - half, dy = y + 0.5f - half;
            float d  = Mathf.Sqrt(dx * dx + dy * dy);
            if (d > half) { px[y * sz + x] = new Color32(0,0,0,0); continue; }
            float ny   = dy / half;
            float band = Mathf.Sin(ny * Mathf.PI * 5f) * 0.5f + 0.5f;
            byte  r = (byte)(30  + band * 40);
            byte  g = (byte)(80  + band * 60);
            byte  b = (byte)(160 + band * 80);
            float limb = Mathf.Sqrt(Mathf.Max(0, 1f - (d / half) * (d / half)));
            r = (byte)(r * limb); g = (byte)(g * limb); b = (byte)(b * limb);
            float hlx = (x - half * 0.65f) / half, hly = (y - half * 1.25f) / half;
            float hl  = Mathf.Max(0f, 1f - (hlx*hlx + hly*hly) * 8f);
            r = (byte)Mathf.Min(255, r + hl * 65);
            g = (byte)Mathf.Min(255, g + hl * 65);
            b = (byte)Mathf.Min(255, b + hl * 45);
            px[y * sz + x] = new Color32(r, g, b, 255);
        }
        tex.SetPixels32(px); tex.Apply(); tex.filterMode = FilterMode.Bilinear;
        return Sprite.Create(tex, new Rect(0, 0, sz, sz), new Vector2(0.5f, 0.5f), sz);
    }
}

