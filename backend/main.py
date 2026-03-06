"""
Mobile Game Factory — Backend API
FastAPI application entry point.
"""

from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from backend.database import init_db
from backend.routers import games, tasks, reviews, blockers

app = FastAPI(
    title="Mobile Game Factory API",
    description="Backend orchestration API for the Mobile Game Factory production system.",
    version="0.1.0",
)

# Allow Next.js dashboard (localhost:3000) to communicate with the API
app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:3000"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Initialize database on startup
@app.on_event("startup")
def startup():
    init_db()

# Register routers
app.include_router(games.router)
app.include_router(tasks.router)
app.include_router(reviews.router)
app.include_router(blockers.router)


@app.get("/", tags=["health"])
def root():
    return {"status": "ok", "service": "Mobile Game Factory API", "version": "0.1.0"}


@app.get("/health", tags=["health"])
def health():
    return {"status": "ok"}
