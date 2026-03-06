"use client";
import { useEffect, useState } from "react";
import { fetchGames, fetchReviews, fetchBlockers, fetchTasks } from "@/lib/api";
import Link from "next/link";

export default function OverviewPage() {
  const [games, setGames] = useState<any[]>([]);
  const [pendingReviews, setPendingReviews] = useState<any[]>([]);
  const [openBlockers, setOpenBlockers] = useState<any[]>([]);
  const [inProgressTasks, setInProgressTasks] = useState<any[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    Promise.all([
      fetchGames(),
      fetchReviews("pending"),
      fetchBlockers("open"),
      fetchTasks(),
    ])
      .then(([g, r, b, t]) => {
        setGames(g);
        setPendingReviews(r);
        setOpenBlockers(b);
        setInProgressTasks(t.filter((t: any) => t.status === "in_progress"));
      })
      .catch(() => setError("Backend is offline. Start the API server."));
  }, []);

  const stats = [
    { label: "Active Games", value: games.length, color: "text-indigo-400" },
    { label: "Pending Reviews", value: pendingReviews.length, color: "text-yellow-400" },
    { label: "Open Blockers", value: openBlockers.length, color: "text-red-400" },
    { label: "Tasks In Progress", value: inProgressTasks.length, color: "text-green-400" },
  ];

  return (
    <div>
      <h1 className="text-2xl font-bold mb-1">Overview</h1>
      <p className="text-gray-400 mb-6 text-sm">Mobile Game Factory - Production Dashboard</p>

      {error && (
        <div className="bg-red-900/40 border border-red-700 text-red-300 rounded px-4 py-3 mb-6 text-sm">
          Backend is offline. Start the API server.
        </div>
      )}

      <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-8">
        {stats.map((s) => (
          <div key={s.label} className="bg-gray-800 rounded-lg p-4">
            <div className={`text-3xl font-bold ${s.color}`}>{s.value}</div>
            <div className="text-gray-400 text-sm mt-1">{s.label}</div>
          </div>
        ))}
      </div>

      <div className="grid md:grid-cols-2 gap-6">
        <div className="bg-gray-800 rounded-lg p-5">
          <div className="flex justify-between items-center mb-4">
            <h2 className="font-semibold">Recent Games</h2>
            <Link href="/games" className="text-indigo-400 text-xs hover:underline">View all</Link>
          </div>
          {games.length === 0 ? (
            <p className="text-gray-500 text-sm">No games yet.</p>
          ) : (
            <ul className="space-y-2">
              {games.slice(0, 5).map((g: any) => (
                <li key={g.id} className="flex justify-between text-sm">
                  <span className="text-gray-100">{g.name}</span>
                  <span className="text-gray-400 text-xs bg-gray-700 px-2 py-0.5 rounded">{g.current_phase}</span>
                </li>
              ))}
            </ul>
          )}
        </div>

        <div className="bg-gray-800 rounded-lg p-5">
          <div className="flex justify-between items-center mb-4">
            <h2 className="font-semibold">Pending Reviews</h2>
            <Link href="/reviews" className="text-indigo-400 text-xs hover:underline">Review Inbox</Link>
          </div>
          {pendingReviews.length === 0 ? (
            <p className="text-gray-500 text-sm">No pending reviews.</p>
          ) : (
            <ul className="space-y-2">
              {pendingReviews.slice(0, 5).map((r: any) => (
                <li key={r.id} className="flex justify-between text-sm">
                  <span className="text-gray-100">{r.review_type}</span>
                  <span className="text-yellow-400 text-xs bg-yellow-900/40 px-2 py-0.5 rounded">pending</span>
                </li>
              ))}
            </ul>
          )}
        </div>

        {openBlockers.length > 0 && (
          <div className="bg-gray-800 rounded-lg p-5 border border-red-800">
            <h2 className="font-semibold text-red-400 mb-4">Open Blockers</h2>
            <ul className="space-y-2">
              {openBlockers.map((b: any) => (
                <li key={b.id} className="text-sm text-gray-300">{b.blocker_reason}</li>
              ))}
            </ul>
          </div>
        )}
      </div>
    </div>
  );
}