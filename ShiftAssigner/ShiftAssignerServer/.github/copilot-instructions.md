Shift Assigner Backend & BDD Testing Instructions

IMPORTANT: Copilot must keep all answers short and to-the-point.

Use this instruction with GitHub Copilot Chat in VS Code.

Overview

You are helping me implement a C# backend for a multi-company shift-assignment system using ASP.NET Core Web API, and helping me write BDD tests using Reqnroll 3.2 and xUnit.

Backend Requirements
Business Rules

System supports multiple companies (multi-tenant).

Each company has its own separate shift data.

Workers schedule preferred shifts (Morning/Day/Evening) for the upcoming week only.

Each day must show all shift slots and indicate available vs filled slots.

Backend Code to Generate
Domain / Entity Classes

Company

Worker (belongs to Company)

ShiftDefinition (Morning/Day/Evening)

ShiftAssignment (Worker + Shift + Date + Status)

DbContext

Clean DbContext with all required DbSet<> items.

Guidelines

Use .NET 8 patterns.

Clear naming and comments.

Enforce tenant isolation.

Show shift-availability logic.

BDD (Reqnroll 3.2 + xUnit)
BDD Artifacts to Generate
1. Feature Scenarios

Worker books Morning shift.

Worker cannot book overlapping shifts.

Worker from Company A cannot access Company B.

Shift at full capacity rejects new bookings.

2. Step Definitions

Use Reqnroll attributes: [Binding], [Given], [When], [Then].

Use in-memory test server: WebApplicationFactory<Program>.

Steps focus on behavior.

3. Reqnroll Setup

Provide example reqnroll.json.

Folder structure: Features/, Steps/, Hooks/.

[BeforeScenario] resets DB/state.

Coding Guidelines

Use modern C# syntax.

Add comments explaining: tenant isolation, shift capacity, availability checks.

Keep structure clean and modular.

Expected Output

Copilot should generate:

Backend models

DbContext

API structures (if needed)

Feature files

Step definitions

Reqnroll config

All in clean, short, and well-structured .NET 8 code.