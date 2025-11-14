You are helping me implement a C# backend for a multi-company shift-assignment system using ASP.NET Core Web API.

Business requirements:

The system must support multiple companies (multi-tenant style).

Each company has its own independent shift-management module (no cross-company data mixing).

Every worker can schedule their preferred shifts (Morning, Day, Evening) for the upcoming week only.

For each day of the upcoming week, the system should present all shift slots and indicate which ones are open and which are already filled.

What I want you to generate now:

C# domain/entity classes for:

Company

Worker (belongs to a Company)

ShiftDefinition (Morning/Day/Evening)

ShiftAssignment (links Worker + ShiftDefinition + Date and stores status)

A DbContext class including DbSet properties for all of the above.