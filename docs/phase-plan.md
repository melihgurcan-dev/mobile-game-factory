# Phase Plan — Mobile Game Factory

## Overview

This document defines the phased development roadmap for the Mobile Game Factory system.
Each phase builds on the previous one and adds operational capability.

---

## Phase 0 — Foundation / System Definition

**Goal:** Define the system, prepare the environment, create the project skeleton.

### Deliverables
- [x] Strategic direction decided (mobile game first, evolve to SaaS)
- [x] System definition written
- [x] Tech stack selected (Unity, Python, Next.js, SQLite, Docker)
- [x] Folder structure created
- [x] `docs/system-blueprint-v1.md` written
- [x] `.gitignore` created
- [x] `README.md` written and encoding fixed
- [ ] `docs/phase-plan.md` filled ← current
- [ ] `docs/agent-hierarchy.md` filled
- [ ] `docs/workflow.md` filled
- [ ] Git initialized + first commit

**Status:** In Progress

---

## Phase 1 — MVP Skeleton

**Goal:** Build a working system skeleton with minimal but functional pieces.

### Deliverables
- [ ] Python backend bootstrapped (FastAPI)
- [ ] SQLite schema defined (games, tasks, agents, reviews, blockers)
- [ ] Basic API endpoints created (`/games`, `/tasks`, `/reviews`)
- [ ] Next.js dashboard initialized
- [ ] Dashboard pages: overview, game list, review inbox
- [ ] Dashboard connects to backend API
- [ ] First dummy game + task + review flow works end to end

**Status:** Not Started

---

## Phase 2 — Agent Integration

**Goal:** Integrate the core AI agents into the production loop.

### Deliverables
- [ ] Orchestrator Agent implemented
- [ ] Game Design Agent implemented
- [ ] Build Agent implemented
- [ ] Content Agent implemented
- [ ] Reporting Agent implemented
- [ ] Agents can create tasks and update game status
- [ ] Dashboard reflects agent activities in real time

**Status:** Not Started

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

| Version | Date       | Notes                        |
|---------|------------|------------------------------|
| 0.1     | 2026-03-06 | Initial phase plan created   |
