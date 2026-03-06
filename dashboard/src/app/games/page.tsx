"use client";
import { useEffect, useState } from "react";
import { fetchGames } from "@/lib/api";

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

  useEffect(() => {
    fetchGames()
      .then(setGames)
      .catch(() => setError("Backend is offline."));
  }, []);

  return (
    <div>
      <h1 className="text-2xl font-bold mb-1">Games</h1>
      <p className="text-gray-400 mb-6 text-sm">All games in the production pipeline.</p>

      {error && (
        <div className="bg-red-900/40 border border-red-700 text-red-300 rounded px-4 py-3 mb-6 text-sm">
          {error}
        </div>
      )}

      {games.length === 0 && !error ? (
        <div className="bg-gray-800 rounded-lg p-8 text-center text-gray-500">
          No games yet. Create the first game to start the pipeline.
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
