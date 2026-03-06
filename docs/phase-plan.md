# Phase Plan — Mobile Game Factory

## Overview

This document defines the phased development roadmap for the Mobile Game Factory system.
Each phase builds on the previous one and adds operational capability.

---

## Phase 0 — Foundation / System Definition ✅

**Goal:** Define the system, prepare the environment, create the project skeleton.

### Deliverables
- [x] Strategic direction decided (mobile game first, evolve to SaaS)
- [x] System definition written
- [x] Tech stack selected (Unity, Python, Next.js, SQLite, Docker)
- [x] Folder structure created
- [x] `docs/system-blueprint-v1.md` written
- [x] `.gitignore` created
- [x] `README.md` written and encoding fixed
- [x] `docs/phase-plan.md` filled
- [x] `docs/agent-hierarchy.md` filled
- [x] `docs/workflow.md` filled
- [x] Git initialized + first commit (2026-03-06)

**Status:** ✅ Complete

---

## Phase 1 — MVP Skeleton 🔄

**Goal:** Build a working system skeleton with minimal but functional pieces.

### Deliverables
- [x] Python backend bootstrapped (FastAPI + uvicorn)
- [x] SQLite schema defined (games, tasks, agents, reviews, blockers)
- [x] Basic API endpoints created (`/games`, `/tasks`, `/reviews`, `/blockers`)
- [x] Python venv configured, dependencies installed
- [x] Database initialized, default agents seeded
- [x] End-to-end API test passed (create game → task → review → blocker)
- [x] Next.js dashboard initialized (TypeScript + Tailwind + App Router)
- [x] Dashboard pages: overview, game list, review inbox, agent status
- [x] API client (`src/lib/api.ts`) connected to backend
- [x] Dashboard connected and running against live backend (2026-03-06)
- [x] First real game can be entered through the dashboard (Create Game form added)

**Status:** ✅ Complete

---

## Phase 2 — Agent Integration ✅

**Goal:** Integrate the core AI agents into the production loop.

### Deliverables
- [x] BaseAgent shared helper layer implemented
- [x] OrchestratorAgent implemented (phase gating, pending-review guard, auto-trigger)
- [x] GameDesignAgent implemented (template MDD per genre, tasks, design_document_approval review)
- [x] BuildAgent implemented (genre-specific Unity task list, build-plan.md, prototype_go_no_go review)
- [x] ContentAgent implemented (YouTube outline, store listing EN+TR, short-form plan, content-package.md)
- [x] Agents create tasks and update game phase in SQLite
- [x] Dashboard game detail page reflects agent pipeline (phase badge, action button, reviews, tasks)
- [x] `/agents/run` endpoint wired to OrchestratorAgent
- [x] Full end-to-end pipeline tested: Stack Jump → idea_pending → design → build → content → published
- [x] Generated artifacts: mini-design-document.md, build-plan.md, content-package.md

**Completed:** 2026-03-06 (v0.4)

**Status:** ✅ Complete

---

## Phase 3 — First Game Pilot

**Goal:** Produce one real mobile game through the system end to end.

### Deliverables
- [ ] One game idea generated and selected
- [ ] Mini design document created by Game Design Agent
- [ ] Prototype task tracked through Build Agent
- [ ] Content package drafted by Content Agent
- [ ] Human review checkpoints passed
- [ ] Game submitted to store (or build exported)
- [ ] YouTube content plan created

**Status:** Not Started

---

## Phase 4 — Content & Publishing Automation

**Goal:** Automate the content creation and publishing pipeline.

### Deliverables
- [ ] Multilingual content generation (EN + TR)
- [ ] Store listing generation
- [ ] YouTube video outline generation
- [ ] Short-form clip plan generation
- [ ] Semi-automated publishing workflow

**Status:** Not Started

---

## Phase 5 — System Stabilization & SaaS Transition Planning

**Goal:** Validate the repeatable system and plan the SaaS evolution.

### Deliverables
- [ ] 4+ games produced through the system
- [ ] Lessons learned documented
- [ ] System reliability reviewed
- [ ] SaaS product definition started
- [ ] Dashboard evolved into multi-project management

**Status:** Not Started

---

## Version History

| Version | Date       | Notes                                              |
|---------|------------|----------------------------------------------------|
| 0.1     | 2026-03-06 | Initial phase plan created                         |
| 0.2     | 2026-03-06 | Phase 0 complete, Phase 1 backend + dashboard done || 0.3     | 2026-03-06 | Phase 1 complete                                   |