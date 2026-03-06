"""Tasks API router."""

from fastapi import APIRouter, HTTPException
from typing import List
from backend.database import get_connection
from backend.models.task import TaskCreate, TaskUpdate

router = APIRouter(prefix="/tasks", tags=["tasks"])


@router.get("/", response_model=List[dict])
def list_tasks(game_id: int = None):
    conn = get_connection()
    if game_id:
        rows = conn.execute(
            "SELECT * FROM tasks WHERE game_id = ? ORDER BY priority, created_at",
            (game_id,),
        ).fetchall()
    else:
        rows = conn.execute(
            "SELECT * FROM tasks ORDER BY priority, created_at"
        ).fetchall()
    conn.close()
    return [dict(row) for row in rows]


@router.post("/", response_model=dict, status_code=201)
def create_task(task: TaskCreate):
    conn = get_connection()
    cursor = conn.execute(
        """INSERT INTO tasks (game_id, agent_name, title, description, priority, review_required)
           VALUES (?, ?, ?, ?, ?, ?) RETURNING *""",
        (task.game_id, task.agent_name, task.title, task.description,
         task.priority, int(task.review_required)),
    )
    row = cursor.fetchone()
    conn.commit()
    conn.close()
    return dict(row)


@router.get("/{task_id}", response_model=dict)
def get_task(task_id: int):
    conn = get_connection()
    row = conn.execute("SELECT * FROM tasks WHERE id = ?", (task_id,)).fetchone()
    conn.close()
    if not row:
        raise HTTPException(status_code=404, detail="Task not found")
    return dict(row)


@router.patch("/{task_id}", response_model=dict)
def update_task(task_id: int, task: TaskUpdate):
    fields = {k: v for k, v in task.model_dump().items() if v is not None}
    if not fields:
        raise HTTPException(status_code=400, detail="No fields to update")
    set_clause = ", ".join(f"{k} = ?" for k in fields)
    set_clause += ", updated_at = datetime('now')"
    values = list(fields.values())
    values.append(task_id)
    conn = get_connection()
    conn.execute(f"UPDATE tasks SET {set_clause} WHERE id = ?", values)
    conn.commit()
    row = conn.execute("SELECT * FROM tasks WHERE id = ?", (task_id,)).fetchone()
    conn.close()
    if not row:
        raise HTTPException(status_code=404, detail="Task not found")
    return dict(row)
