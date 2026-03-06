# Stack Jump — Unity Scene Setup Guide

**Unity Version:** 6000.3.10f1  
**Project Path:** `games/stack-jump/unity-project/`

---

## Quick Start (5 minutes)

### 1. Open the Project
1. Open **Unity Hub**
2. Click **Open → Add project from disk**
3. Navigate to `games/stack-jump/unity-project/` and click **Open**
4. Unity will import packages (first launch: ~2 min)

---

## 2. Create the Game Scene

1. **File → New Scene → Basic 2D** → Save as `Assets/Scenes/Game`
2. Delete the default `Main Camera` — we'll set up our own

---

## 3. Scene Hierarchy Setup

Create the following GameObjects:

### A — `_GameManager` (Empty Object, at origin)
Attach components:
- `GameManager`
- `StackController` → assign **Main Camera** field
- `ScoreManager`
- `AudioManager` → assign audio clips when available
- `JuiceEffects` → assign **Target Camera**

### B — `Main Camera`
- **Transform:** Position `(0, 5, -10)`, Rotation `(0, 0, 0)`  
- **Camera:** Projection = Orthographic, Size = 7  
- **Background color:** `#1A1A2E` (dark navy)

### C — `Canvas` (UI)
- **Canvas Scaler:** UI Scale Mode = Scale With Screen Size  
  Reference Resolution: **1080 × 1920** (portrait mobile)
- Attach `UIManager` component to the Canvas

#### Inside Canvas, create:

| Name | Type | Notes |
|------|------|-------|
| `ScoreText` | TextMeshPro – Text | centered top, font size 80 |
| `BestText` | TextMeshPro – Text | below score, font size 36 |
| `MenuPanel` | Panel | full-screen overlay |
| `GameOverPanel` | Panel | full-screen overlay, hidden by default |

**MenuPanel** children:
- `Title` — "STACK JUMP" (TMP, large bold)
- `BestScoreText` — "Best: 0" (TMP)
- `PlayButton` — Button → OnClick: `UIManager.OnPlayButton()`

**GameOverPanel** children:
- `ScoreLabel` — "SCORE" (TMP, small)
- `ScoreText` — final score value (TMP, large bold)
- `BestText` — best score (TMP, medium)
- `NewBestText` — "NEW BEST!" (TMP, yellow, hidden by default)
- `RetryButton` — Button → OnClick: `UIManager.OnRetryButton()`

### D — Wire UIManager References (Inspector)
Select the Canvas → UIManager component:
- `Score Text` → `ScoreText`
- `Best Text` → `BestText`
- `Menu Panel` → `MenuPanel`
- `Menu Best Score Text` → `MenuPanel/BestScoreText`
- `Game Over Panel` → `GameOverPanel`
- `Game Over Score Text` → `GameOverPanel/ScoreText`
- `Game Over Best Text` → `GameOverPanel/BestText`
- `Game Over New Best Text` → `GameOverPanel/NewBestText`

---

## 4. First Run Checklist

- [ ] Press **Play** → Menu panel visible, "STACK JUMP" title shown  
- [ ] Click **Play button** → base block visible at center  
- [ ] Click anywhere → moving block slides in, stops on click  
- [ ] Partially overlap → block is cut, score increases  
- [ ] Miss completely → "GAME OVER" panel appears  
- [ ] Retry → scene reloads cleanly  

---

## 5. Build Settings (Android)

1. **File → Build Settings → Android**
2. Click **Switch Platform**
3. **Player Settings:**
   - Company Name: MobileGameFactory
   - Product Name: Stack Jump
   - Orientation: Portrait (locked)
   - Minimum API: 24 (Android 7.0)
   - Target API: 34 (Android 14)
4. **Build → Build APK** → output to `Builds/Android/`

---

## 6. Script Reference

| Script | Attach To | Purpose |
|--------|-----------|---------|
| `GameManager.cs` | `_GameManager` | State machine, input |
| `StackController.cs` | `_GameManager` | Core stacking logic |
| `MovingBlock.cs` | Auto-added by StackController | Block movement |
| `ScoreManager.cs` | `_GameManager` | Score + PlayerPrefs |
| `UIManager.cs` | `Canvas` | All UI panels |
| `AudioManager.cs` | `_GameManager` | SFX + music |
| `GameColors.cs` | Static utility | Block color palette |
| `JuiceEffects.cs` | `_GameManager` | Particles + camera shake |

---

## 7. Gameplay Tuning (StackController Inspector)

| Parameter | Default | Effect |
|-----------|---------|--------|
| `initialBlockWidth` | 3.0 | Starting platform width |
| `blockHeight` | 0.25 | Height of each block |
| `moveRange` | 3.5 | How far blocks travel L/R |
| `initialSpeed` | 2.5 | Starting movement speed |
| `speedIncreasePerBlock` | 0.04 | Speed ramp per block |
| `maxSpeed` | 7.0 | Speed cap |

---

## 8. Next Steps

- [ ] Add SFX clips (tap, perfect, fail) to AudioManager
- [ ] Add background music loop
- [ ] Add `ParticleSystem` prefabs to JuiceEffects
- [ ] Replace `CreatePrimitive` blocks with 2D sprites for better visuals
- [ ] Integrate banner + rewarded ad SDK (AdMob/AppLovin)
- [ ] Add Google Play Games leaderboard support
