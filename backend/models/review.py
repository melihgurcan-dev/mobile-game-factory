"""Pydantic models for Review entity."""

from pydantic import BaseModel
from typing import Optional


class ReviewCreate(BaseModel):
    game_id: int
    task_id: Optional[int] = None
    review_type: str
    notes: Optional[str] = None


class ReviewUpdate(BaseModel):
    status: Optional[str] = None
    notes: Optional[str] = None


class ReviewOut(BaseModel):
    id: int
    game_id: int
    task_id: Optional[int]
    review_type: str
    status: str
    notes: Optional[str]
    created_at: str
