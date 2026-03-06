"use client";
import { useEffect, useState } from "react";

const API_BASE = process.env.NEXT_PUBLIC_API_URL || "http://localhost:8000";

const AGENT_DESCRIPTIONS: Record<string, string> = {
  orchestrator: "Manages workflow, assigns tasks, tracks phases, flags blockers.",
  game_design: "Generates game ideas, defines core loop, creates mini design docs.",
  build: "Handles prototype implementation tasks, tracks build progress.",
  content: "Prepares YouTube/store/video content tasks, drafts content assets.",
  reporting: "Sends structured updates to dashboard, creates review tasks.",
};

export default function AgentsPage() {
  const [agents, setAgents] = useState<any[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetch(`${API_BASE}/tasks`)
      .then((r) => r.json())
      .then(async () => {
        // Fetch agents via the DB indirectly — we derive from tasks
        // For now fetch a static list from known agents
        const res = await fetch(`${API_BASE}/tasks`);
        const tasks = await res.json();
        const agentNames = [
          "orchestrator",
          "game_design",
          "build",
          "content",
          "reporting",
        ];
        const agentStats = agentNames.map((name) => {
          const agentTasks = tasks.filter((t: any) => t.agent_name === name);
          const inProgress = agentTasks.filter(
            (t: any) => t.status === "in_progress"
          ).length;
          const blocked = agentTasks.filter(
            (t: any) => t.blocker_flag === 1
          ).length;
          return {
            name,
            total: agentTasks.length,
            inProgress,
            blocked,
            status: inProgress > 0 ? "active" : "idle",
          };
        });
        setAgents(agentStats);
      })
      .catch(() => setError("Backend is offline."));
  }, []);

  const statusBadge = (s: string) => {
    return s === "active"
      ? "bg-green-900 text-green-300"
      : "bg-gray-700 text-gray-400";
  };

  return (
    <div>
      <h1 className="text-2xl font-bold mb-1">Agent Status</h1>
      <p className="text-gray-400 mb-6 text-sm">
        Overview of all production agents and their current workload.
      </p>

      {error && (
        <div className="bg-red-900/40 border border-red-700 text-red-300 rounded px-4 py-3 mb-6 text-sm">
          {error}
        </div>
      )}

      <div className="grid md:grid-cols-2 gap-4">
        {agents.map((a) => (
          <div key={a.name} className="bg-gray-800 rounded-lg p-5">
            <div className="flex justify-between items-start mb-2">
              <div>
                <div className="font-medium text-gray-100 capitalize">
                  {a.name.replace("_", " ")} Agent
                </div>
                <div className="text-gray-500 text-xs mt-0.5">
                  {AGENT_DESCRIPTIONS[a.name]}
                </div>
              </div>
              <span className={`text-xs px-2 py-1 rounded ${statusBadge(a.status)}`}>
                {a.status}
              </span>
            </div>
            <div className="flex gap-4 mt-3 text-xs text-gray-400">
              <span>Total tasks: <strong className="text-gray-200">{a.total}</strong></span>
              <span>In progress: <strong className="text-indigo-400">{a.inProgress}</strong></span>
              {a.blocked > 0 && (
                <span>Blocked: <strong className="text-red-400">{a.blocked}</strong></span>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
