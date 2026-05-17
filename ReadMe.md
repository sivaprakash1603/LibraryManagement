
# Library Management System (Console)

A sample 3-layer .NET 8.0 console application demonstrating a Community Library Management System using Entity Framework Core and PostgreSQL.

## Overview

This project implements a simple library management system with a console-based front-end and a layered architecture:
- `LibraryManagementModelLibrary` — Domain models and enums
- `LibraryManagementDALLibrary` — EF Core DbContext and repositories (data access)
- `LibraryManagementBLLibrary` — Business logic and services
- `LibraryManagementFEApplication` — Console UI (front-end)

Key features:
- Member management (add, search, update status, deactivate)
- Book management (add books, add copies, search, mark damaged/unavailable)
- Borrowing workflow (borrow, return) with business rules and PostgreSQL function integration
- Fine management (calculate, pay, history)
- Reports (currently borrowed, overdue, most borrowed, available by category, member history)

## Tech Stack

- .NET 8.0 (C#)
- Entity Framework Core (Npgsql provider)
- PostgreSQL

## Prerequisites

- .NET 8 SDK installed
- PostgreSQL running locally (default connection uses `postgres/postgres`)

Update the connection string in `LibraryManagementDALLibrary/Contexts/LibraryContext.cs` if needed.

## Setup & Run

1. Restore and build the solution:

```bash
dotnet build
```

2. Apply EF Core migrations and seed data (run from repository root):

```bash
dotnet ef database update --project LibraryManagementDALLibrary --startup-project LibraryManagementFEApplication
```

3. Run the console application:

```bash
cd LibraryManagementFEApplication
dotnet run
```

Follow the on-screen menu to manage members, books, borrowings, fines and reports.

## Notes & Important Details

- The application uses PostgreSQL stored functions for some operations (e.g., creating borrowings and calculating fines). Avoid manual disposal of the DbContext's connection; the repositories use EF Core APIs or carefully manage connection Open/Close.
- Membership types are seeded via migrations (Basic, Student, Premium). If you encounter FK errors when adding members, ensure migrations have been applied.
- Currency values in the UI use the ₹ symbol (Indian Rupee) and are formatted to two decimal places.
