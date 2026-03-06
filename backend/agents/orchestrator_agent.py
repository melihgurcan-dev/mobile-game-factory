"""
Orchestrator Agent — manages pipeline phases and delegates to specialized agents.
"""

from backend.agents.base_agent import BaseAgent
from backend.agents.game_design_agent import GameDesignAgent
from backend.agents.build_agent import BuildAgent
from backend.agents.content_agent import ContentAgent


class OrchestratorAgent(BaseAgent):
    name = "orchestrator"

    def start_design_phase(self, game_id: int) -> dict:
        """Kick off the design phase for a game."""
        game = self.get_game(game_id)
        if not game:
            raise ValueError(f"Game {game_id} not found")

        allowed_phases = ["idea_pending", "design_in_progress"]
        if game["current_phase"] not in allowed_phases:
            raise ValueError(
                f"Game is in phase '{game['current_phase']}', cannot start design phase."
            )

        print(f"\n[orchestrator] Starting pipeline for: {game['name']} (id={game_id})")
        result = GameDesignAgent().run(game_id)
        print(f"[orchestrator] Design phase started. Review ID: {result['review_id']}")
        return result

    def advance_phase(self, game_id: int) -> dict:
        """
        Advance game to next phase and auto-run the corresponding agent.
        design_in_progress → build_in_progress  (runs BuildAgent)
        build_in_progress  → content_in_progress (runs ContentAgent)
        content_in_progress → publishing_review
        publishing_review  → published
        """
        game = self.get_game(game_id)
        if not game:
            raise ValueError(f"Game {game_id} not found")

        current = game["current_phase"]

        # Check no pending reviews before advancing
        conn = __import__("backend.database", fromlist=["get_connection"]).get_connection()
        pending = conn.execute(
            "SELECT COUNT(*) FROM reviews WHERE game_id = ? AND status = 'pending'",
            (game_id,),
        ).fetchone()[0]
        conn.close()
        if pending > 0:
            raise ValueError(
                f"There are {pending} pending review(s) for this game. "
                "Approve or reject them before advancing."
            )

        phase_map = {
            "design_in_progress": "build_in_progress",
            "build_in_progress":  "content_in_progress",
            "content_in_progress": "publishing_review",
            "publishing_review":  "published",
        }

        next_phase = phase_map.get(current)
        if not next_phase:
            raise ValueError(f"No next phase defined for '{current}'")

        # Update phase
        self.update_game_phase(game_id, next_phase)
        print(f"[orchestrator] {current} -> {next_phase}")

        result: dict = {"game_id": game_id, "previous_phase": current, "new_phase": next_phase}

        # Auto-run next agent
        if next_phase == "build_in_progress":
            agent_result = BuildAgent().run(game_id)
            result["agent_action"] = agent_result

        elif next_phase == "content_in_progress":
            agent_result = ContentAgent().run(game_id)
            result["agent_action"] = agent_result

        elif next_phase == "published":
            self.update_game_field(game_id, "publish_status", "published")
            self.update_game_field(game_id, "build_status", "completed")
            self.update_game_field(game_id, "content_status", "completed")
            print(f"[orchestrator] Game {game_id} published!")

        return result
