"""
Build Agent — breaks a game design into implementation tasks and tracks prototype progress.
Template-based for MVP. Unity-specific tasks are generated from the MDD genre.
"""

from pathlib import Path
from backend.agents.base_agent import BaseAgent

GAMES_DIR = Path(__file__).parent.parent.parent / "games"

# Task templates per genre
GENRE_BUILD_TASKS = {
    "casual": [
        ("Set up Unity project (2D, mobile settings)", "Create new Unity project. Set build target to Android + iOS. Configure resolution and orientation.", 1),
        ("Implement core mechanic", "Build the primary tap/swipe interaction. Add basic physics or collision logic.", 1),
        ("Implement score system", "Add score counter. Display live score on screen. Store local high score via PlayerPrefs.", 2),
        ("Add difficulty scaling", "Increase game speed or obstacle frequency over time. Define difficulty curve.", 2),
        ("Implement Game Over screen", "Show score on death. Add Retry and Main Menu buttons.", 2),
        ("Build main menu", "Simple scene with Play button. Show high score. Add basic UI polish.", 3),
        ("Add audio: SFX + background music", "Add tap sound, fail sound, and looping background music. Use AudioSource components.", 3),
        ("Add juice effects", "Particle effects on score milestone. Screen shake on fail. Add tweening for UI elements.", 3),
        ("Build & test on device", "Build APK/IPA. Test on physical device. Fix any platform-specific issues.", 4),
    ],
    "puzzle": [
        ("Set up Unity project (2D, mobile settings)", "Create new Unity project. Set build target to Android + iOS.", 1),
        ("Design level data structure", "Define how level data is stored (ScriptableObjects or JSON). Create 5 test levels.", 1),
        ("Implement core puzzle mechanic", "Build drag/tap interaction for puzzle pieces. Add snap logic.", 1),
        ("Implement level completion logic", "Detect win state. Animate completion. Load next level.", 2),
        ("Build level select screen", "Show completed/locked levels. Persist progress via PlayerPrefs.", 2),
        ("Add hint system", "Implement limited hints. Add rewarded ad hook for extra hints.", 3),
        ("Add audio: SFX + ambient", "Relaxing background loop. Tap sounds. Completion fanfare.", 3),
        ("Build & test on device", "Build APK/IPA. Test on physical device.", 4),
    ],
    "arcade": [
        ("Set up Unity project (2D, mobile settings)", "Create project with mobile build targets.", 1),
        ("Implement player controller", "Tap-to-action mechanic. Add responsive controls.", 1),
        ("Implement obstacle/enemy spawner", "Random or pattern-based spawning. Increase spawn rate over time.", 1),
        ("Implement collision & fail state", "Detect collisions. Trigger game over. Show score.", 2),
        ("Add score + combo system", "Track score and combos. Display on HUD.", 2),
        ("Add power-ups", "Define 2–3 power-up types. Implement pick-up logic.", 3),
        ("Add audio & visual feedback", "SFX for actions. Particles on hit/pickup.", 3),
        ("Build & test on device", "APK/IPA build. Physical device test.", 4),
    ],
    "runner": [
        ("Set up Unity project (2D, mobile settings)", "Mobile build targets. Landscape or portrait decision.", 1),
        ("Implement endless runner movement", "Auto-scroll world or move character. Set base speed.", 1),
        ("Implement jump mechanic", "Tap to jump. Double-jump optional. Adjust gravity feel.", 1),
        ("Implement obstacle generation", "Random obstacle spawner. Recycle objects with object pooling.", 2),
        ("Add distance/score tracking", "Track distance run. Convert to score. Show on HUD.", 2),
        ("Add character animations", "Run, jump, fall, die animations. Animator controller setup.", 3),
        ("Add parallax backgrounds", "Multi-layer scrolling background. Increase scroll speed over time.", 3),
        ("Build & test on device", "APK/IPA build. Physical device test.", 4),
    ],
}

DEFAULT_BUILD_TASKS = [
    ("Set up Unity project (2D, mobile settings)", "Create Unity project, set Android + iOS build targets.", 1),
    ("Implement core mechanic", "Build the primary player interaction.", 1),
    ("Implement score / progression system", "Track and display player progress.", 2),
    ("Implement fail state and Game Over screen", "Detect loss condition. Show retry screen.", 2),
    ("Build main menu", "Simple start screen with Play button.", 3),
    ("Add audio (SFX + music)", "Basic sound effects and background music.", 3),
    ("Build & test on device", "Export APK/IPA and test on physical device.", 4),
]


def _get_build_tasks(genre: str | None) -> list[tuple]:
    if not genre:
        return DEFAULT_BUILD_TASKS
    return GENRE_BUILD_TASKS.get(genre.lower().strip(), DEFAULT_BUILD_TASKS)


class BuildAgent(BaseAgent):
    name = "build"

    def run(self, game_id: int) -> dict:
        """
        Full build phase setup:
        1. Load game
        2. Create implementation tasks from template
        3. Create build progress tracking file
        4. Create human go/no-go review
        5. Update game fields
        """
        game = self.get_game(game_id)
        if not game:
            raise ValueError(f"Game {game_id} not found")

        print(f"\n[build] Starting build phase for: {game['name']}")

        # 1. Generate tasks
        raw_tasks = _get_build_tasks(game.get("genre"))
        created_task_ids = []

        for title, description, priority in raw_tasks:
            task = self.create_task(
                game_id,
                title=title,
                description=description,
                priority=priority,
                review_required=(priority == 4),  # Final build task requires review
            )
            created_task_ids.append(task["id"])

        # 2. Save build plan file
        plan_path = self._save_build_plan(game["name"], game.get("genre"), raw_tasks)

        # 3. Update game build_status
        self.update_game_field(game_id, "build_status", "in_progress")

        # 4. Create go/no-go review
        review = self.create_review(
            game_id,
            review_type="prototype_go_no_go",
            notes=f"Prototype build tasks created for '{game['name']}'. Review the task list and approve to proceed with implementation.",
        )

        print(f"[build] {len(created_task_ids)} build tasks created.")
        return {
            "status": "build_phase_started",
            "game_id": game_id,
            "game_name": game["name"],
            "build_plan_path": str(plan_path),
            "tasks_created": created_task_ids,
            "review_id": review["id"],
        }

    def _save_build_plan(self, game_name: str, genre: str | None, tasks: list[tuple]) -> Path:
        slug = game_name.lower().replace(" ", "-")
        game_dir = GAMES_DIR / slug
        game_dir.mkdir(parents=True, exist_ok=True)

        lines = [
            f"# Build Plan: {game_name}\n",
            f"**Genre:** {genre or 'N/A'}  \n",
            "**Status:** In Progress  \n",
            "**Agent:** Build Agent (template-based)\n\n",
            "---\n\n",
            "## Implementation Tasks\n\n",
        ]

        priority_groups: dict[int, list] = {}
        for title, desc, prio in tasks:
            priority_groups.setdefault(prio, []).append((title, desc))

        priority_labels = {1: "Sprint 1 — Core", 2: "Sprint 2 — Gameplay", 3: "Sprint 3 — Polish", 4: "Sprint 4 — Build & QA"}
        for prio in sorted(priority_groups):
            lines.append(f"### {priority_labels.get(prio, f'Priority {prio}')}\n\n")
            for title, desc in priority_groups[prio]:
                lines.append(f"- [ ] **{title}**  \n  {desc}\n\n")

        lines.append("---\n\n*Generated by Build Agent. Review and adjust before implementation.*\n")

        plan_path = game_dir / "build-plan.md"
        plan_path.write_text("".join(lines), encoding="utf-8")
        return plan_path
