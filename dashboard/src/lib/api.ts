const API_BASE = process.env.NEXT_PUBLIC_API_URL || "http://localhost:8000";

export async function fetchGames() {
  const res = await fetch(`${API_BASE}/games/`);
  if (!res.ok) throw new Error("Failed to fetch games");
  return res.json();
}

export async function fetchGame(id: number) {
  const res = await fetch(`${API_BASE}/games/${id}`);
  if (!res.ok) throw new Error("Failed to fetch game");
  return res.json();
}

export async function fetchTasks(gameId?: number) {
  const url = gameId
    ? `${API_BASE}/tasks/?game_id=${gameId}`
    : `${API_BASE}/tasks/`;
  const res = await fetch(url);
  if (!res.ok) throw new Error("Failed to fetch tasks");
  return res.json();
}

export async function fetchReviews(status?: string) {
  const url = status
    ? `${API_BASE}/reviews/?status=${status}`
    : `${API_BASE}/reviews/`;
  const res = await fetch(url);
  if (!res.ok) throw new Error("Failed to fetch reviews");
  return res.json();
}

export async function fetchBlockers(status?: string) {
  const url = status
    ? `${API_BASE}/blockers/?status=${status}`
    : `${API_BASE}/blockers/`;
  const res = await fetch(url);
  if (!res.ok) throw new Error("Failed to fetch blockers");
  return res.json();
}

export async function fetchGameReviews(gameId: number) {
  const res = await fetch(`${API_BASE}/reviews/?game_id=${gameId}`);
  if (!res.ok) throw new Error("Failed to fetch reviews");
  return res.json();
}

export async function updateReview(
  id: number,
  payload: { status: string; notes?: string }
) {
  const res = await fetch(`${API_BASE}/reviews/${id}`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });
  if (!res.ok) throw new Error("Failed to update review");
  return res.json();
}

export async function runAgent(action: string, gameId: number) {
  const res = await fetch(`${API_BASE}/agents/run`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ action, game_id: gameId }),
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail || "Agent action failed");
  }
  return res.json();
}
