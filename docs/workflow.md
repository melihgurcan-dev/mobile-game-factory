# Workflow — Mobile Game Factory

## Overview

This document describes the weekly production workflow for the Mobile Game Factory system.
Each game follows this workflow from idea to archive.

---

## Weekly Production Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                     WEEKLY GAME PIPELINE                        │
│                                                                 │
│  1. Idea Generation                                             │
│       └─ Game Design Agent generates 3-5 ideas                 │
│                                                                 │
│  2. Idea Selection  [HUMAN REVIEW]                              │
│       └─ Human picks 1 idea from dashboard                      │
│                                                                 │
│  3. Mini Design Document                                        │
│       └─ Game Design Agent creates MDD                         │
│                                                                 │
│  4. Design Approval  [HUMAN REVIEW]                             │
│       └─ Human approves or requests revisions                   │
│                                                                 │
│  5. Prototype Task Creation                                     │
│       └─ Build Agent breaks design into tasks                   │
│                                                                 │
│  6. Build Progress Tracking                                     │
│       └─ Build Agent tracks Unity implementation                │
│                                                                 │
│  7. Prototype Go/No-Go  [HUMAN REVIEW]                          │
│       └─ Human decides to continue or pivot                     │
│                                                                 │
│  8. Content Pack Preparation                                    │
│       └─ Content Agent creates full content package             │
│                                                                 │
│  9. Content Direction Approval  [HUMAN REVIEW]                  │
│       └─ Human approves content direction                       │
│                                                                 │
│ 10. Final Review  [HUMAN REVIEW]                                │
│       └─ Human approves for publishing                          │
│                                                                 │
│ 11. Publish / Archive                                           │
│       └─ Game published, week archived                          │
└─────────────────────────────────────────────────────────────────┘
```

---

## Game Status States

Each game moves through the following statuses:

| Status              | Description                                      |
|---------------------|--------------------------------------------------|
| `idea_pending`      | Waiting for human to select an idea              |
| `design_in_progress`| Game Design Agent is creating the MDD            |
| `design_review`     | Waiting for human design approval                |
| `build_in_progress` | Build Agent is tracking prototype implementation |
| `build_review`      | Waiting for human go/no-go decision              |
| `content_in_progress`| Content Agent is building the content package   |
| `content_review`    | Waiting for human content approval               |
| `publishing_review` | Waiting for final human publishing approval      |
| `published`         | Game and content published                       |
| `archived`          | Week complete and archived                       |
| `blocked`           | Workflow blocked — requires human attention      |
| `cancelled`         | Game cancelled, not proceeding                   |

---

## Task States

Each task within a game follows these states:

| Status        | Description                              |
|---------------|------------------------------------------|
| `pending`     | Task created, not yet started            |
| `in_progress` | Agent is actively working on task        |
| `review`      | Task needs human review                  |
| `approved`    | Human approved task output               |
| `rejected`    | Human rejected — task needs revision     |
| `blocked`     | Task is blocked by a dependency or issue |
| `completed`   | Task fully completed                     |
| `cancelled`   | Task cancelled                           |

---

## Blocker Handling

When a blocker is detected:

1. The responsible agent flags the blocker via the Orchestrator
2. The Orchestrator creates a blocker entry in the database
3. The Reporting Agent pushes the blocker to the dashboard
4. The dashboard shows the blocker in the "Blocked Tasks" view
5. Human operator reviews and resolves or escalates
6. Once resolved, the Orchestrator resumes the workflow

---

## Human Review Handling

When a human review is required:

1. The responsible agent marks the task as `review`
2. The Reporting Agent creates a review inbox item
3. The dashboard shows the review in the "Review Inbox"
4. Human operator reviews, then approves or rejects
5. If approved → workflow continues automatically
6. If rejected → agent receives feedback and revises

---

## Content Package Definition

Each game produces the following content package:

| Asset                    | Owner         | Language |
|--------------------------|---------------|----------|
| YouTube video outline    | Content Agent | EN       |
| YouTube short-form plan  | Content Agent | EN + TR  |
| Store listing (title)    | Content Agent | EN + TR  |
| Store listing (desc)     | Content Agent | EN + TR  |
| Thumbnail concept brief  | Content Agent | N/A      |
| Keywords / ASO tags      | Content Agent | EN + TR  |

---

## MVP Simplifications

For Phase 1 MVP, the following simplifications apply:

- Agents do not run autonomously; tasks are created manually or via scripts
- Workflow transitions are triggered manually or via API calls
- Content package is a structured document, not auto-generated
- Publishing is not automated; export is manual

Full autonomous workflow is a Phase 2+ goal.

---

## Version History

| Version | Date       | Notes                         |
|---------|------------|-------------------------------|
| 0.1     | 2026-03-06 | Initial workflow doc created  |
