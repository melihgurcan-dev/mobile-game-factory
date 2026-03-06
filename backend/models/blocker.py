"""Pydantic models for Blocker entity."""

from pydantic import BaseModel
from typing import Optional


class BlockerCreate(BaseModel):
    game_id: int
    task_id: Optional[int] = None
    blocker_reason: str


class BlockerUpdate(BaseModel):
    status: Optional[str] = None


class BlockerOut(BaseModel):
    id: int
    game_id: int
    task_id: Optional[int]
    blocker_reason: str
    status: str
    created_at: str
