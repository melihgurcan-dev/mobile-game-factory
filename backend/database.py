"""
Database setup for Mobile Game Factory.
Uses SQLite via Python's built-in sqlite3 for MVP simplicity.
"""

import sqlite3
import os
from pathlib import Path

DB_PATH = Path(__file__).parent / "factory.db"


def get_connection() -> sqlite3.Connection:
    """Return a SQLite connection with row_factory set."""
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    conn.execute("PRAGMA foreign_keys = ON")
    return conn


def init_db():
    """Create all tables if they don't exist."""
    conn = get_connection()
    cursor = conn.cursor()

    cursor.executescript("""
        CREATE TABLE IF NOT EXISTS games (
            id              INTEGER PRIMARY KEY AUTOINCREMENT,
            name            TEXT NOT NULL,
            genre           TEXT,
            current_phase   TEXT NOT NULL DEFAULT 'idea_pending',
            build_status    TEXT NOT NULL DEFAULT 'pending',
            content_status  TEXT NOT NULL DEFAULT 'pending',
            publish_status  TEXT NOT NULL DEFAULT 'pending',
            created_at      TEXT NOT NULL DEFAULT (datetime('now')),
            updated_at      TEXT NOT NULL DEFAULT (datetime('now'))
        );

        CREATE TABLE IF NOT EXISTS agents (
            id          INTEGER PRIMARY KEY AUTOINCREMENT,
            name        TEXT NOT NULL UNIQUE,
            role        TEXT NOT NULL,
            status      TEXT NOT NULL DEFAULT 'idle',
            current_game_id INTEGER REFERENCES games(id),
            created_at  TEXT NOT NULL DEFAULT (datetime('now')),
            updated_at  TEXT NOT NULL DEFAULT (datetime('now'))
        );

        CREATE TABLE IF NOT EXISTS tasks (
            id              INTEGER PRIMARY KEY AUTOINCREMENT,
            game_id         INTEGER NOT NULL REFERENCES games(id),
            agent_name      TEXT NOT NULL,
            title           TEXT NOT NULL,
            description     TEXT,
            status          TEXT NOT NULL DEFAULT 'pending',
            priority        INTEGER NOT NULL DEFAULT 1,
            review_required INTEGER NOT NULL DEFAULT 0,
            blocker_flag    INTEGER NOT NULL DEFAULT 0,
            created_at      TEXT NOT NULL DEFAULT (datetime('now')),
            updated_at      TEXT NOT NULL DEFAULT (datetime('now'))
        );

        CREATE TABLE IF NOT EXISTS reviews (
            id          INTEGER PRIMARY KEY AUTOINCREMENT,
            game_id     INTEGER NOT NULL REFERENCES games(id),
            task_id     INTEGER REFERENCES tasks(id),
            review_type TEXT NOT NULL,
            status      TEXT NOT NULL DEFAULT 'pending',
            notes       TEXT,
            created_at  TEXT NOT NULL DEFAULT (datetime('now'))
        );

        CREATE TABLE IF NOT EXISTS blockers (
            id              INTEGER PRIMARY KEY AUTOINCREMENT,
            game_id         INTEGER NOT NULL REFERENCES games(id),
            task_id         INTEGER REFERENCES tasks(id),
            blocker_reason  TEXT NOT NULL,
            status          TEXT NOT NULL DEFAULT 'open',
            created_at      TEXT NOT NULL DEFAULT (datetime('now'))
        );
    """)

    # Seed default agents if not present
    cursor.execute("SELECT COUNT(*) FROM agents")
    if cursor.fetchone()[0] == 0:
        agents = [
            ("orchestrator", "Orchestrator Agent"),
            ("game_design", "Game Design Agent"),
            ("build", "Build Agent"),
            ("content", "Content Agent"),
            ("reporting", "Reporting Agent"),
        ]
        cursor.executemany(
            "INSERT INTO agents (name, role) VALUES (?, ?)", agents
        )

    conn.commit()
    conn.close()
    print(f"[DB] Database initialized at {DB_PATH}")


if __name__ == "__main__":
    init_db()
