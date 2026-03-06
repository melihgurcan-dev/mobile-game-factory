"""
Orchestrator Agent — manages pipeline phases and delegates to specialized agents.
"""

from backend.agents.base_agent import BaseAgent
from backend.agents.game_design_agent import GameDesignAgent


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
        agent = GameDesignAgent()
        result = agent.run(game_id)
        print(f"[orchestrator] Design phase started. Review ID: {result['review_id']}")
        return result

    def advance_phase(self, game_id: int) -> dict:
        """Advance game to next phase after all reviews are approved."""
        game = self.get_game(game_id)
        if not game:
            raise ValueError(f"Game {game_id} not found")

        phase_map = {
            "design_in_progress": "build_in_progress",
            "build_in_progress": "content_in_progress",
            "content_in_progress": "publishing_review",
            "publishing_review": "published",
        }

        current = game["current_phase"]
        next_phase = phase_map.get(current)
        if not next_phase:
            raise ValueError(f"No next phase defined for '{current}'")

        self.update_game_phase(game_id, next_phase)
        print(f"[orchestrator] Game {game_id} advanced: {current} → {next_phase}")
        return {"game_id": game_id, "previous_phase": current, "new_phase": next_phase}
