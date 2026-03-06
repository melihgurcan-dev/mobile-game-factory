"""Games API router."""

from fastapi import APIRouter, HTTPException
from typing import List
from backend.database import get_connection
from backend.models.game import GameCreate, GameUpdate, GameOut

router = APIRouter(prefix="/games", tags=["games"])


@router.get("/", response_model=List[dict])
def list_games():
    conn = get_connection()
    rows = conn.execute("SELECT * FROM games ORDER BY created_at DESC").fetchall()
    conn.close()
    return [dict(row) for row in rows]


@router.post("/", response_model=dict, status_code=201)
def create_game(game: GameCreate):
    conn = get_connection()
    cursor = conn.execute(
        "INSERT INTO games (name, genre) VALUES (?, ?) RETURNING *",
        (game.name, game.genre),
    )
    row = cursor.fetchone()
    conn.commit()
    conn.close()
    return dict(row)


@router.get("/{game_id}", response_model=dict)
def get_game(game_id: int):
    conn = get_connection()
    row = conn.execute("SELECT * FROM games WHERE id = ?", (game_id,)).fetchone()
    conn.close()
    if not row:
        raise HTTPException(status_code=404, detail="Game not found")
    return dict(row)


@router.patch("/{game_id}", response_model=dict)
def update_game(game_id: int, game: GameUpdate):
    fields = {k: v for k, v in game.model_dump().items() if v is not None}
    if not fields:
        raise HTTPException(status_code=400, detail="No fields to update")
    fields["updated_at"] = "datetime('now')"
    set_clause = ", ".join(f"{k} = ?" for k in fields if k != "updated_at")
    set_clause += ", updated_at = datetime('now')"
    values = [v for k, v in fields.items() if k != "updated_at"]
    values.append(game_id)
    conn = get_connection()
    conn.execute(f"UPDATE games SET {set_clause} WHERE id = ?", values)
    conn.commit()
    row = conn.execute("SELECT * FROM games WHERE id = ?", (game_id,)).fetchone()
    conn.close()
    if not row:
        raise HTTPException(status_code=404, detail="Game not found")
    return dict(row)


@router.delete("/{game_id}", status_code=204)
def delete_game(game_id: int):
    conn = get_connection()
    conn.execute("DELETE FROM games WHERE id = ?", (game_id,))
    conn.commit()
    conn.close()
