# Shift Assigner Backend & BDD Testing Instructions

Use this instruction with GitHub Copilot Chat in VS Code.

---

## Overview

You are helping me implement a **C# backend** for a **multi-company shift‑assignment system** using **ASP.NET Core Web API**, and also helping me write **BDD tests** using **Reqnroll 3.2** and **xUnit**.

---

# Backend Requirements

## Business Rules

1. The system must support **multiple companies** (multi‑tenant architecture).
2. Each company has its own **independent shift‑management module** (no cross‑company data mixing).
3. Workers can schedule their preferred shifts:

   * Morning
   * Day
   * Evening

   Only for the **upcoming week**.
4. For each day, the system must show all shift slots and clearly indicate:

   * Available slots
   * Already filled slots

---

## Backend Code to Generate

Copilot should generate:

### **Domain / Entity Classes**

* `Company`
* `Worker` (belongs to a `Company`)
* `ShiftDefinition` (Morning/Day/Evening)
* `ShiftAssignment` (Worker + Shift + Date + status)

### **DbContext**

* A clean `DbContext` with all `DbSet<>` entries

### **Guidelines**

* Use .NET 8 patterns
* Clear naming & comments
* Enforce multi‑tenant separation
* Demonstrate shift availability logic

---

# BDD (Reqnroll 3.2 + xUnit) Requirements

You are also assisting in creating BDD tests.

## BDD Artifacts to Generate

### **1. .feature File Scenarios**

* Worker successfully books a Morning shift for an upcoming day
* Worker cannot book overlapping shifts on the same day
* Worker from Company A cannot view/book Company B’s shifts (tenant isolation)
* Shift with max capacity rejects additional bookings

### **2. Step Definitions (C#)**

* Use Reqnroll 3.2 attributes:

  * `[Binding]`
  * `[Given]`
  * `[When]`
  * `[Then]`
* Use in‑memory test server:

  * `WebApplicationFactory<Program>`
* Steps must focus on **behavior** (BDD style)

### **3. Reqnroll Setup**

* Example `reqnroll.json`
* Folder structure:

  * `Features/`
  * `Steps/`
  * `Hooks/`
* Include `[BeforeScenario]` hook to reset DB/state

---

## Coding Guidelines

* Use modern C# syntax
* Add comments explaining:

  * Multi‑tenant isolation
  * Shift capacity rules
  * Availability checks
* Keep structure clean, modular, and understandable

---

## Expected Output

Copilot should produce:

* Backend models
* DbContext
* API structures (if needed)
* Reqnroll `.feature` files
* Step definitions
* Reqnroll configuration

All in clean, well‑structured .NET 8 code.

---
