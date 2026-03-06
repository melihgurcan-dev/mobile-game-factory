"""
Base Agent — shared helpers for all agents.
"""

from datetime import datetime
from backend.database import get_connection


class BaseAgent:
    name: str = "base"

    # ------------------------------------------------------------------ #
    # Game helpers
    # ------------------------------------------------------------------ #
    def get_game(self, game_id: int) -> dict | None:
        conn = get_connection()
        row = conn.execute("SELECT * FROM games WHERE id = ?", (game_id,)).fetchone()
        conn.close()
        return dict(row) if row else None

    def update_game_phase(self, game_id: int, phase: str) -> None:
        conn = get_connection()
        conn.execute(
            "UPDATE games SET current_phase = ?, updated_at = datetime('now') WHERE id = ?",
            (phase, game_id),
        )
        conn.commit()
        conn.close()
        print(f"[{self.name}] Game {game_id} phase → {phase}")

    def update_game_field(self, game_id: int, field: str, value: str) -> None:
        conn = get_connection()
        conn.execute(
            f"UPDATE games SET {field} = ?, updated_at = datetime('now') WHERE id = ?",
            (value, game_id),
        )
        conn.commit()
        conn.close()

    # ------------------------------------------------------------------ #
    # Task helpers
    # ------------------------------------------------------------------ #
    def create_task(
        self,
        game_id: int,
        title: str,
        description: str = "",
        priority: int = 1,
        review_required: bool = False,
    ) -> dict:
        conn = get_connection()
        cursor = conn.execute(
            """INSERT INTO tasks (game_id, agent_name, title, description, priority, review_required)
               VALUES (?, ?, ?, ?, ?, ?) RETURNING *""",
            (game_id, self.name, title, description, priority, int(review_required)),
        )
        row = cursor.fetchone()
        conn.commit()
        conn.close()
        task = dict(row)
        print(f"[{self.name}] Task created: [{task['id']}] {task['title']}")
        return task

    def update_task_status(self, task_id: int, status: str) -> None:
        conn = get_connection()
        conn.execute(
            "UPDATE tasks SET status = ?, updated_at = datetime('now') WHERE id = ?",
            (status, task_id),
        )
        conn.commit()
        conn.close()

    # ------------------------------------------------------------------ #
    # Review helpers
    # ------------------------------------------------------------------ #
    def create_review(
        self,
        game_id: int,
        review_type: str,
        task_id: int | None = None,
        notes: str = "",
    ) -> dict:
        conn = get_connection()
        cursor = conn.execute(
            """INSERT INTO reviews (game_id, task_id, review_type, notes)
               VALUES (?, ?, ?, ?) RETURNING *""",
            (game_id, task_id, review_type, notes),
        )
        row = cursor.fetchone()
        conn.commit()
        conn.close()
        review = dict(row)
        print(f"[{self.name}] Review created: [{review['id']}] {review_type}")
        return review

    # ------------------------------------------------------------------ #
    # Blocker helpers
    # ------------------------------------------------------------------ #
    def create_blocker(
        self,
        game_id: int,
        reason: str,
        task_id: int | None = None,
    ) -> dict:
        conn = get_connection()
        cursor = conn.execute(
            """INSERT INTO blockers (game_id, task_id, blocker_reason)
               VALUES (?, ?, ?) RETURNING *""",
            (game_id, task_id, reason),
        )
        row = cursor.fetchone()
        conn.commit()
        conn.close()
        blocker = dict(row)
        print(f"[{self.name}] Blocker created: {reason}")
        return blocker
