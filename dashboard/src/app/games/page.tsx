"use client";
import { useEffect, useState } from "react";
import { fetchGames } from "@/lib/api";

const API_BASE = process.env.NEXT_PUBLIC_API_URL || "http://localhost:8000";

async function createGame(name: string, genre: string) {
  const res = await fetch(`${API_BASE}/games/`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ name, genre: genre || null }),
  });
  if (!res.ok) throw new Error("Failed to create game");
  return res.json();
}

const PHASE_COLORS: Record<string, string> = {
  idea_pending: "bg-gray-700 text-gray-300",
  design_in_progress: "bg-blue-900 text-blue-300",
  design_review: "bg-yellow-900 text-yellow-300",
  build_in_progress: "bg-indigo-900 text-indigo-300",
  build_review: "bg-yellow-900 text-yellow-300",
  content_in_progress: "bg-purple-900 text-purple-300",
  content_review: "bg-yellow-900 text-yellow-300",
  publishing_review: "bg-orange-900 text-orange-300",
  published: "bg-green-900 text-green-300",
  archived: "bg-gray-700 text-gray-500",
  blocked: "bg-red-900 text-red-300",
  cancelled: "bg-gray-700 text-gray-500",
};

export default function GamesPage() {
  const [games, setGames] = useState<any[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [name, setName] = useState("");
  const [genre, setGenre] = useState("");
  const [creating, setCreating] = useState(false);

  const load = () => fetchGames().then(setGames).catch(() => setError("Backend is offline."));

  useEffect(() => { load(); }, []);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) return;
    setCreating(true);
    try {
      await createGame(name.trim(), genre.trim());
      setName("");
      setGenre("");
      setShowForm(false);
      load();
    } catch {
      setError("Failed to create game.");
    } finally {
      setCreating(false);
    }
  };

  return (
    <div>
      <div className="flex justify-between items-center mb-1">
        <h1 className="text-2xl font-bold">Games</h1>
        <button
          onClick={() => setShowForm((v) => !v)}
          className="text-sm px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded transition-colors"
        >
          {showForm ? "Cancel" : "+ New Game"}
        </button>
      </div>
      <p className="text-gray-400 mb-6 text-sm">All games in the production pipeline.</p>

      {error && (
        <div className="bg-red-900/40 border border-red-700 text-red-300 rounded px-4 py-3 mb-6 text-sm">
          {error}
        </div>
      )}

      {showForm && (
        <form
          onSubmit={handleCreate}
          className="bg-gray-800 rounded-lg p-5 mb-6 border border-indigo-700"
        >
          <h2 className="font-semibold mb-4 text-indigo-300">New Game</h2>
          <div className="flex gap-3">
            <input
              type="text"
              placeholder="Game name *"
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="flex-1 bg-gray-700 text-gray-100 placeholder-gray-500 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
              required
            />
            <input
              type="text"
              placeholder="Genre (optional)"
              value={genre}
              onChange={(e) => setGenre(e.target.value)}
              className="w-48 bg-gray-700 text-gray-100 placeholder-gray-500 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
            <button
              type="submit"
              disabled={creating || !name.trim()}
              className="px-5 py-2 bg-green-700 hover:bg-green-600 disabled:opacity-50 text-white rounded text-sm transition-colors"
            >
              {creating ? "Creating..." : "Create"}
            </button>
          </div>
        </form>
      )}

      {games.length === 0 && !error ? (
        <div className="bg-gray-800 rounded-lg p-8 text-center text-gray-500">
          No games yet. Click <strong>+ New Game</strong> to start the pipeline.
        </div>
      ) : (
        <div className="space-y-3">
          {games.map((g: any) => (
            <div key={g.id} className="bg-gray-800 rounded-lg p-5 flex items-center justify-between">
              <div>
                <div className="font-medium text-gray-100">{g.name}</div>
                <div className="text-gray-500 text-xs mt-0.5">
                  {g.genre || "No genre"} · Created {g.created_at?.slice(0, 10)}
                </div>
              </div>
              <div className="flex gap-2 items-center">
                <span className={`text-xs px-2 py-1 rounded ${PHASE_COLORS[g.current_phase] || "bg-gray-700 text-gray-300"}`}>
                  {g.current_phase}
                </span>
                <span className="text-xs px-2 py-1 rounded bg-gray-700 text-gray-400">
                  build: {g.build_status}
                </span>
                <span className="text-xs px-2 py-1 rounded bg-gray-700 text-gray-400">
                  content: {g.content_status}
                </span>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
