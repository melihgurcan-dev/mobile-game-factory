"use client";
import { useEffect, useState } from "react";
import { fetchReviews, updateReview } from "@/lib/api";

export default function ReviewsPage() {
  const [reviews, setReviews] = useState<any[]>([]);
  const [filter, setFilter] = useState("pending");
  const [error, setError] = useState<string | null>(null);

  const load = () => {
    fetchReviews(filter === "all" ? undefined : filter)
      .then(setReviews)
      .catch(() => setError("Backend is offline."));
  };

  useEffect(() => { load(); }, [filter]);

  const handleAction = async (id: number, status: "approved" | "rejected") => {
    await updateReview(id, { status });
    load();
  };

  const statusBadge = (s: string) => {
    const map: Record<string, string> = {
      pending: "bg-yellow-900 text-yellow-300",
      approved: "bg-green-900 text-green-300",
      rejected: "bg-red-900 text-red-300",
    };
    return map[s] || "bg-gray-700 text-gray-400";
  };

  return (
    <div>
      <h1 className="text-2xl font-bold mb-1">Review Inbox</h1>
      <p className="text-gray-400 mb-6 text-sm">Human-in-the-loop approval queue.</p>

      {error && (
        <div className="bg-red-900/40 border border-red-700 text-red-300 rounded px-4 py-3 mb-6 text-sm">
          {error}
        </div>
      )}

      <div className="flex gap-2 mb-6">
        {["pending", "approved", "rejected", "all"].map((f) => (
          <button
            key={f}
            onClick={() => setFilter(f)}
            className={`text-xs px-3 py-1.5 rounded transition-colors ${
              filter === f ? "bg-indigo-600 text-white" : "bg-gray-700 text-gray-300 hover:bg-gray-600"
            }`}
          >
            {f}
          </button>
        ))}
      </div>

      {reviews.length === 0 ? (
        <div className="bg-gray-800 rounded-lg p-8 text-center text-gray-500">
          No reviews with status: {filter}
        </div>
      ) : (
        <div className="space-y-3">
          {reviews.map((r: any) => (
            <div key={r.id} className="bg-gray-800 rounded-lg p-5">
              <div className="flex items-start justify-between">
                <div>
                  <div className="font-medium text-gray-100">{r.review_type}</div>
                  <div className="text-gray-500 text-xs mt-0.5">
                    Game ID: {r.game_id} {r.task_id ? `· Task ID: ${r.task_id}` : ""}
                    {" · "}{r.created_at?.slice(0, 16)}
                  </div>
                  {r.notes && (
                    <div className="text-gray-400 text-sm mt-2 bg-gray-700/50 rounded p-2">
                      {r.notes}
                    </div>
                  )}
                </div>
                <div className="flex items-center gap-2 ml-4 shrink-0">
                  <span className={`text-xs px-2 py-1 rounded ${statusBadge(r.status)}`}>
                    {r.status}
                  </span>
                  {r.status === "pending" && (
                    <>
                      <button
                        onClick={() => handleAction(r.id, "approved")}
                        className="text-xs px-3 py-1 bg-green-800 hover:bg-green-700 text-green-200 rounded transition-colors"
                      >
                        Approve
                      </button>
                      <button
                        onClick={() => handleAction(r.id, "rejected")}
                        className="text-xs px-3 py-1 bg-red-800 hover:bg-red-700 text-red-200 rounded transition-colors"
                      >
                        Reject
                      </button>
                    </>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
