# Agent Hierarchy — Mobile Game Factory

## Overview

This document defines the agent structure for the Mobile Game Factory system.
Agents are modular, semi-autonomous units that handle specific parts of the production workflow.
All agents report to the Orchestrator, which is the single source of task truth.

---

## Agent Map

```
Human Operator
     │
     ▼
Orchestrator Agent
     │
     ├──► Game Design Agent
     ├──► Build Agent
     ├──► Content Agent
     └──► Reporting Agent
```

---

## Agent Definitions

### 1. Orchestrator Agent

**Role:** Central workflow controller

**Responsibilities:**
- Manages the overall game production pipeline
- Assigns tasks to specialized agents
- Tracks phase progress across all active games
- Flags blockers and escalates to human operator
- Ensures human review checkpoints are respected
- Maintains the task queue

**Inputs:**
- Human operator decisions
- Agent status updates
- Review outcomes

**Outputs:**
- Task assignments
- Phase transitions
- Blocker flags
- Dashboard state updates

---

### 2. Game Design Agent

**Role:** Idea generation and game design

**Responsibilities:**
- Generates multiple game idea candidates
- Structures core game loop definitions
- Creates mini game design documents
- Proposes genre, mechanic, and target audience
- Prepares the design brief for the Build Agent

**Inputs:**
- Orchestrator instructions
- Game genre/theme guidelines
- Previous game history (to avoid duplication)

**Outputs:**
- Game idea list
- Selected game concept
- Mini design document (MDD)

**Human Review Required:** Yes — idea selection and design approval

---

### 3. Build Agent

**Role:** Prototype implementation and build tracking

**Responsibilities:**
- Breaks down the game design into implementation tasks
- Tracks Unity build progress
- Flags technical blockers
- Reports build status to Orchestrator
- Prepares the build for QA / export

**Inputs:**
- Mini design document from Game Design Agent
- Orchestrator task assignments

**Outputs:**
- Implementation task list
- Build status reports
- Blocker reports
- Final build artifact reference

**Human Review Required:** Yes — go/no-go decision on prototype

---

### 4. Content Agent

**Role:** Content asset preparation

**Responsibilities:**
- Creates YouTube video outline
- Generates short-form clip plan
- Drafts store listing copy (EN + TR)
- Prepares thumbnail concept briefs
- Creates multilingual content package

**Inputs:**
- Game design document
- Build status (game is playable/exported)
- Orchestrator task assignments

**Outputs:**
- YouTube content outline
- Short-form clip plan
- Store listing draft (EN + TR)
- Content package summary

**Human Review Required:** Yes — content direction approval

---

### 5. Reporting Agent

**Role:** Status aggregation and dashboard updates

**Responsibilities:**
- Aggregates status from all agents
- Sends structured updates to the dashboard
- Creates review tasks for human operator
- Generates weekly production summaries
- Flags stalled workflows

**Inputs:**
- Status reports from all agents
- Task completion events
- Blocker events

**Outputs:**
- Dashboard updates
- Review inbox items
- Weekly summary reports

**Human Review Required:** No (reporting only)

---

## Human-in-the-Loop Review Points

| Review Point              | Triggered By         | Blocking? |
|---------------------------|----------------------|-----------|
| Game idea selection       | Game Design Agent    | Yes       |
| Design document approval  | Game Design Agent    | Yes       |
| Prototype go/no-go        | Build Agent          | Yes       |
| Content direction approval| Content Agent        | Yes       |
| Publishing decision       | Reporting Agent      | Yes       |
| Optimization approval     | Reporting Agent      | Optional  |

---

## MVP Agent Scope

For Phase 1 MVP, agents will be simulated as lightweight Python modules,
not full LLM-powered autonomous agents. Full LLM agent integration is Phase 2+.

---

## Version History

| Version | Date       | Notes                              |
|---------|------------|------------------------------------|
| 0.1     | 2026-03-06 | Initial agent hierarchy created    |
