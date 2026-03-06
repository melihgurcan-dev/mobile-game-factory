"""Agent runner API router — triggers agent actions from the dashboard."""

from fastapi import APIRouter, HTTPException
from pydantic import BaseModel
from backend.agents.orchestrator_agent import OrchestratorAgent

router = APIRouter(prefix="/agents", tags=["agents"])
orchestrator = OrchestratorAgent()


class RunRequest(BaseModel):
    action: str   # "start_design_phase" | "advance_phase"
    game_id: int


@router.post("/run", response_model=dict)
def run_agent(req: RunRequest):
    """Trigger an orchestrator action for a game."""
    try:
        if req.action == "start_design_phase":
            return orchestrator.start_design_phase(req.game_id)
        elif req.action == "advance_phase":
            return orchestrator.advance_phase(req.game_id)
        else:
            raise HTTPException(status_code=400, detail=f"Unknown action: {req.action}")
    except ValueError as e:
        raise HTTPException(status_code=400, detail=str(e))


@router.get("/", response_model=list)
def list_agents():
    """Return all agents and their current workload."""
    from backend.database import get_connection
    conn = get_connection()
    tasks = conn.execute("SELECT agent_name, status, blocker_flag FROM tasks").fetchall()
    conn.close()

    agent_names = ["orchestrator", "game_design", "build", "content", "reporting"]
    result = []
    for name in agent_names:
        agent_tasks = [t for t in tasks if t["agent_name"] == name]
        result.append({
            "name": name,
            "total_tasks": len(agent_tasks),
            "in_progress": sum(1 for t in agent_tasks if t["status"] == "in_progress"),
            "blocked": sum(1 for t in agent_tasks if t["blocker_flag"] == 1),
            "status": "active" if any(t["status"] == "in_progress" for t in agent_tasks) else "idle",
        })
    return result
