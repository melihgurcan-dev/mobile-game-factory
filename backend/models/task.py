"""Pydantic models for Task entity."""

from pydantic import BaseModel
from typing import Optional


class TaskCreate(BaseModel):
    game_id: int
    agent_name: str
    title: str
    description: Optional[str] = None
    priority: int = 1
    review_required: bool = False


class TaskUpdate(BaseModel):
    title: Optional[str] = None
    description: Optional[str] = None
    status: Optional[str] = None
    priority: Optional[int] = None
    review_required: Optional[bool] = None
    blocker_flag: Optional[bool] = None


class TaskOut(BaseModel):
    id: int
    game_id: int
    agent_name: str
    title: str
    description: Optional[str]
    status: str
    priority: int
    review_required: bool
    blocker_flag: bool
    created_at: str
    updated_at: str
