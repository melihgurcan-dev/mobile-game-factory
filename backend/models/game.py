"""Pydantic models for Game entity."""

from pydantic import BaseModel
from typing import Optional


class GameCreate(BaseModel):
    name: str
    genre: Optional[str] = None


class GameUpdate(BaseModel):
    name: Optional[str] = None
    genre: Optional[str] = None
    current_phase: Optional[str] = None
    build_status: Optional[str] = None
    content_status: Optional[str] = None
    publish_status: Optional[str] = None


class GameOut(BaseModel):
    id: int
    name: str
    genre: Optional[str]
    current_phase: str
    build_status: str
    content_status: str
    publish_status: str
    created_at: str
    updated_at: str
