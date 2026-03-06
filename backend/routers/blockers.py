"""Blockers API router."""

from fastapi import APIRouter, HTTPException
from typing import List
from backend.database import get_connection
from backend.models.blocker import BlockerCreate, BlockerUpdate

router = APIRouter(prefix="/blockers", tags=["blockers"])


@router.get("/", response_model=List[dict])
def list_blockers(status: str = None):
    conn = get_connection()
    if status:
        rows = conn.execute(
            "SELECT * FROM blockers WHERE status = ? ORDER BY created_at DESC",
            (status,),
        ).fetchall()
    else:
        rows = conn.execute(
            "SELECT * FROM blockers ORDER BY created_at DESC"
        ).fetchall()
    conn.close()
    return [dict(row) for row in rows]


@router.post("/", response_model=dict, status_code=201)
def create_blocker(blocker: BlockerCreate):
    conn = get_connection()
    cursor = conn.execute(
        """INSERT INTO blockers (game_id, task_id, blocker_reason)
           VALUES (?, ?, ?) RETURNING *""",
        (blocker.game_id, blocker.task_id, blocker.blocker_reason),
    )
    row = cursor.fetchone()
    conn.commit()
    conn.close()
    return dict(row)


@router.patch("/{blocker_id}", response_model=dict)
def update_blocker(blocker_id: int, blocker: BlockerUpdate):
    if not blocker.status:
        raise HTTPException(status_code=400, detail="No fields to update")
    conn = get_connection()
    conn.execute(
        "UPDATE blockers SET status = ? WHERE id = ?",
        (blocker.status, blocker_id),
    )
    conn.commit()
    row = conn.execute("SELECT * FROM blockers WHERE id = ?", (blocker_id,)).fetchone()
    conn.close()
    if not row:
        raise HTTPException(status_code=404, detail="Blocker not found")
    return dict(row)
