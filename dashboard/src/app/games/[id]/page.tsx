"use client";
import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { fetchGame, fetchTasks, fetchReviews, fetchBlockers, runAgent, updateReview } from "@/lib/api";
import Link from "next/link";

const PHASE_COLORS: Record<string, string> = {
  idea_pending:         "bg-gray-700 text-gray-300",
  design_in_progress:   "bg-blue-900 text-blue-300",
  design_review:        "bg-yellow-900 text-yellow-300",
  build_in_progress:    "bg-indigo-900 text-indigo-300",
  build_review:         "bg-yellow-900 text-yellow-300",
  content_in_progress:  "bg-purple-900 text-purple-300",
  content_review:       "bg-yellow-900 text-yellow-300",
  publishing_review:    "bg-orange-900 text-orange-300",
  published:            "bg-green-900 text-green-300",
  blocked:              "bg-red-900 text-red-300",
};

const STATUS_COLORS: Record<string, string> = {
  pending:      "bg-gray-700 text-gray-400",
  in_progress:  "bg-indigo-900 text-indigo-300",
  review:       "bg-yellow-900 text-yellow-300",
  approved:     "bg-green-900 text-green-300",
  rejected:     "bg-red-900 text-red-300",
  blocked:      "bg-red-900 text-red-300",
  completed:    "bg-green-900 text-green-300",
};

const PHASE_ACTIONS: Record<string, { label: string; action: string } | null> = {
  idea_pending:       { label: "Start Design Phase", action: "start_design_phase" },
  design_in_progress: { label: "Advance to Build Phase", action: "advance_phase" },
  build_in_progress:  { label: "Advance to Content Phase", action: "advance_phase" },
  content_in_progress:{ label: "Advance to Publishing Review", action: "advance_phase" },
  publishing_review:  { label: "Mark as Published", action: "advance_phase" },
  published:          null,
};

export default function GameDetailPage() {
  const { id } = useParams<{ id: string }>();
  const gameId = parseInt(id);
  const router = useRouter();

  const [game, setGame] = useState<any>(null);
  const [tasks, setTasks] = useState<any[]>([]);
  const [reviews, setReviews] = useState<any[]>([]);
  const [blockers, setBlockers] = useState<any[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [running, setRunning] = useState(false);
  const [actionMsg, setActionMsg] = useState<string | null>(null);

  const load = async () => {
    try {
      const [g, t, r, b] = await Promise.all([
        fetchGame(gameId),
        fetchTasks(gameId),
        fetchReviews(),
        fetchBlockers(),
      ]);
      setGame(g);
      setTasks(t);
      setReviews(r.filter((x: any) => x.game_id === gameId));
      setBlockers(b.filter((x: any) => x.game_id === gameId && x.status === "open"));
    } catch {
      setError("Backend is offline.");
    }
  };

  useEffect(() => { load(); }, [gameId]);

  const handleAgentAction = async (action: string) => {
    setRunning(true);
    setActionMsg(null);
    try {
      const res = await runAgent(action, gameId);
      setActionMsg(`Done: ${res.status || res.new_phase || "success"}`);
      load();
    } catch (e: any) {
      setActionMsg(`Error: ${e.message}`);
    } finally {
      setRunning(false);
    }
  };

  const handleReview = async (reviewId: number, status: "approved" | "rejected") => {
    await updateReview(reviewId, { status });
    load();
  };

  if (!game && !error) {
    return <div className="text-gray-500 text-sm">Loading...</div>;
  }

  const phaseAction = game ? PHASE_ACTIONS[game.current_phase] : null;
  const pendingReviews = reviews.filter((r: any) => r.status === "pending");

  return (
    <div>
      {/* Header */}
      <div className="flex items-center gap-2 mb-1 text-sm text-gray-500">
        <Link href="/games" className="hover:text-gray-300">Games</Link>
        <span>/</span>
        <span className="text-gray-300">{game?.name}</span>
      </div>

      <div className="flex justify-between items-start mb-6">
        <div>
          <h1 className="text-2xl font-bold">{game?.name}</h1>
          <p className="text-gray-500 text-sm mt-0.5">
            {game?.genre || "No genre"} · Created {game?.created_at?.slice(0, 10)}
          </p>
        </div>
        {game && (
          <span className={`text-sm px-3 py-1 rounded ${PHASE_COLORS[game.current_phase] || "bg-gray-700 text-gray-300"}`}>
            {game.current_phase}
          </span>
        )}
      </div>

      {error && (
        <div className="bg-red-900/40 border border-red-700 text-red-300 rounded px-4 py-3 mb-6 text-sm">
          {error}
        </div>
      )}

      {/* Agent Action Button */}
      {phaseAction && (
        <div className="bg-gray-800 border border-indigo-800 rounded-lg p-5 mb-6">
          <div className="flex items-center justify-between">
            <div>
              <div className="font-medium text-indigo-300">Next Agent Action</div>
              <div className="text-gray-400 text-sm mt-0.5">
                Run the Orchestrator to proceed to the next pipeline step.
              </div>
            </div>
            <button
              onClick={() => handleAgentAction(phaseAction.action)}
              disabled={running}
              className="px-5 py-2 bg-indigo-600 hover:bg-indigo-500 disabled:opacity-50 text-white rounded text-sm font-medium transition-colors"
            >
              {running ? "Running..." : phaseAction.label}
            </button>
          </div>
          {actionMsg && (
            <div className="mt-3 text-sm text-indigo-300 bg-indigo-900/30 rounded px-3 py-2">
              {actionMsg}
            </div>
          )}
        </div>
      )}

      {/* Pending Reviews */}
      {pendingReviews.length > 0 && (
        <div className="bg-gray-800 border border-yellow-800 rounded-lg p-5 mb-6">
          <h2 className="font-semibold text-yellow-300 mb-3">
            Pending Reviews ({pendingReviews.length})
          </h2>
          <div className="space-y-3">
            {pendingReviews.map((r: any) => (
              <div key={r.id} className="flex items-start justify-between bg-gray-700/50 rounded p-3">
                <div>
                  <div className="font-medium text-sm text-gray-100">{r.review_type}</div>
                  {r.notes && <div className="text-gray-400 text-xs mt-1">{r.notes}</div>}
                </div>
                <div className="flex gap-2 ml-4 shrink-0">
                  <button
                    onClick={() => handleReview(r.id, "approved")}
                    className="text-xs px-3 py-1 bg-green-800 hover:bg-green-700 text-green-200 rounded"
                  >
                    Approve
                  </button>
                  <button
                    onClick={() => handleReview(r.id, "rejected")}
                    className="text-xs px-3 py-1 bg-red-800 hover:bg-red-700 text-red-200 rounded"
                  >
                    Reject
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Open Blockers */}
      {blockers.length > 0 && (
        <div className="bg-gray-800 border border-red-800 rounded-lg p-5 mb-6">
          <h2 className="font-semibold text-red-400 mb-3">Open Blockers</h2>
          <ul className="space-y-2">
            {blockers.map((b: any) => (
              <li key={b.id} className="text-sm text-gray-300 bg-gray-700/50 rounded px-3 py-2">
                {b.blocker_reason}
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Status Grid */}
      {game && (
        <div className="grid grid-cols-3 gap-3 mb-6">
          {[
            { label: "Build", value: game.build_status },
            { label: "Content", value: game.content_status },
            { label: "Publish", value: game.publish_status },
          ].map((s) => (
            <div key={s.label} className="bg-gray-800 rounded-lg p-4 text-center">
              <div className="text-gray-500 text-xs mb-1">{s.label}</div>
              <div className={`text-xs px-2 py-1 rounded inline-block ${STATUS_COLORS[s.value] || "bg-gray-700 text-gray-400"}`}>
                {s.value}
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Tasks */}
      <div className="bg-gray-800 rounded-lg p-5 mb-6">
        <h2 className="font-semibold mb-4">
          Tasks <span className="text-gray-500 font-normal text-sm">({tasks.length})</span>
        </h2>
        {tasks.length === 0 ? (
          <p className="text-gray-500 text-sm">No tasks yet. Run an agent to generate tasks.</p>
        ) : (
          <div className="space-y-2">
            {tasks.map((t: any) => (
              <div key={t.id} className="flex items-start justify-between bg-gray-700/50 rounded px-4 py-3">
                <div className="flex-1 min-w-0">
                  <div className="text-sm font-medium text-gray-100">{t.title}</div>
                  {t.description && (
                    <div className="text-gray-500 text-xs mt-0.5 truncate">{t.description.split("\n")[0]}</div>
                  )}
                  <div className="text-gray-600 text-xs mt-0.5">
                    Agent: {t.agent_name} · Priority: {t.priority}
                    {t.review_required ? " · Review required" : ""}
                    {t.blocker_flag ? " · ⛔ Blocked" : ""}
                  </div>
                </div>
                <span className={`ml-3 shrink-0 text-xs px-2 py-1 rounded ${STATUS_COLORS[t.status] || "bg-gray-700 text-gray-400"}`}>
                  {t.status}
                </span>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Review History */}
      {reviews.length > 0 && (
        <div className="bg-gray-800 rounded-lg p-5">
          <h2 className="font-semibold mb-4">
            Review History <span className="text-gray-500 font-normal text-sm">({reviews.length})</span>
          </h2>
          <div className="space-y-2">
            {reviews.map((r: any) => (
              <div key={r.id} className="flex justify-between items-center bg-gray-700/50 rounded px-4 py-2 text-sm">
                <span className="text-gray-300">{r.review_type}</span>
                <span className={`text-xs px-2 py-1 rounded ${STATUS_COLORS[r.status] || "bg-gray-700 text-gray-400"}`}>
                  {r.status}
                </span>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
