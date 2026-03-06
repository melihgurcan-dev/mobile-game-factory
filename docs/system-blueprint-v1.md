# System Blueprint v1

## 1. System Definition
Mobile Game Factory is a semi-autonomous production system that creates one simple mobile game per week, prepares multilingual content assets for each game, and makes all agent activities visible through a dashboard.

## 2. Primary Objective
The primary objective is to build a repeatable production machine that can:
- generate game ideas
- select viable concepts
- create prototype tasks
- prepare content packs
- route review tasks to human approval
- make progress visible from a central dashboard

## 3. Human Role
The human operator reviews and approves critical decisions through the dashboard.

Human review points:
- game idea selection
- prototype go / no-go
- content direction
- publishing decision
- optimization approval

## 4. Initial MVP Scope
The initial MVP will include:
- one game pipeline
- a minimal dashboard
- a small set of core agents
- task tracking
- review inbox
- basic status updates

## 5. Core Agents
### Orchestrator Agent
Manages workflow, assigns tasks, tracks progress, flags blockers.

### Game Design Agent
Generates game ideas, core loop definitions, and mini design documents.

### Build Agent
Owns prototype implementation tasks and build progress.

### Content Agent
Prepares YouTube/video/store content tasks and draft assets.

### Reporting Agent
Feeds structured updates into the dashboard and creates review tasks.

## 6. Weekly Production Flow
1. Idea generation
2. Idea selection
3. Mini design document
4. Prototype task creation
5. Build progress tracking
6. Content pack preparation
7. Human review
8. Finalize / publish / archive

## 7. Dashboard Modules
- Game list
- Current phase
- Agent status
- Review inbox
- Blocked tasks
- Game detail page

## 8. Data To Track
- game name
- genre
- current phase
- responsible agent
- blocker status
- review required
- content status
- build status
- publish status

## 9. MVP Technology Stack
- Unity for game development
- Python for backend and orchestration
- Next.js for dashboard
- SQLite for initial data storage
- Docker for environment consistency

## 10. Success Criteria for MVP
- a game entry can be created
- agent tasks can be assigned
- dashboard can display current state
- human review can be requested
- one prototype workflow can be tracked end-to-end