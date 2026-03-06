"""Reviews API router."""

from fastapi import APIRouter, HTTPException
from typing import List
from backend.database import get_connection
from backend.models.review import ReviewCreate, ReviewUpdate

router = APIRouter(prefix="/reviews", tags=["reviews"])


@router.get("/", response_model=List[dict])
def list_reviews(status: str = None):
    conn = get_connection()
    if status:
        rows = conn.execute(
            "SELECT * FROM reviews WHERE status = ? ORDER BY created_at DESC",
            (status,),
        ).fetchall()
    else:
        rows = conn.execute(
            "SELECT * FROM reviews ORDER BY created_at DESC"
        ).fetchall()
    conn.close()
    return [dict(row) for row in rows]


@router.post("/", response_model=dict, status_code=201)
def create_review(review: ReviewCreate):
    conn = get_connection()
    cursor = conn.execute(
        """INSERT INTO reviews (game_id, task_id, review_type, notes)
           VALUES (?, ?, ?, ?) RETURNING *""",
        (review.game_id, review.task_id, review.review_type, review.notes),
    )
    row = cursor.fetchone()
    conn.commit()
    conn.close()
    return dict(row)


@router.patch("/{review_id}", response_model=dict)
def update_review(review_id: int, review: ReviewUpdate):
    fields = {k: v for k, v in review.model_dump().items() if v is not None}
    if not fields:
        raise HTTPException(status_code=400, detail="No fields to update")
    set_clause = ", ".join(f"{k} = ?" for k in fields)
    values = list(fields.values())
    values.append(review_id)
    conn = get_connection()
    conn.execute(f"UPDATE reviews SET {set_clause} WHERE id = ?", values)
    conn.commit()
    row = conn.execute("SELECT * FROM reviews WHERE id = ?", (review_id,)).fetchone()
    conn.close()
    if not row:
        raise HTTPException(status_code=404, detail="Review not found")
    return dict(row)
