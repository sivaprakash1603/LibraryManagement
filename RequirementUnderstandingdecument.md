# Requirement Understanding Document

## Project Title
Community Library Management System

## Purpose
The goal of this project is to build a console-based library management system for a community library. The application should support daily library operations such as managing members, books, book copies, borrowing, returning, fine tracking, and report generation.

## High-Level Objective
The system should provide a simple, reliable, and modular solution that follows a 3-layer architecture:

- Model Library for entity definitions
- DAL Library for database access
- BLL Library for business rules and validation
- FE Console Application for user interaction

## Stakeholders

- Library staff who manage members and books
- Library administrators who need reports and transaction tracking
- Developers maintaining or extending the application

## Scope

### In Scope

- Add, view, search, update, and deactivate members
- Add books and book copies
- Search books by title, author, and category
- Borrow and return books
- Enforce business rules such as membership limits, active membership checks, fine limits, and copy availability
- Track and display fines
- Generate reports for borrowing and availability
- Persist data in PostgreSQL using Entity Framework Core

### Out of Scope

- Web or mobile user interface
- Barcode scanning hardware integration
- Authentication and role-based login system
- Online payments or third-party payment gateway integration
- Notifications by email, SMS, or push messages

## Functional Requirements

### Member Management

- The system shall allow adding a new member.
- The system shall allow viewing all members.
- The system shall allow searching members by phone number.
- The system shall allow searching members by email address.
- The system shall allow updating membership status.
- The system shall allow deactivating a member.
- The system shall assign a membership type to each member.

### Book Management

- The system shall allow adding a new book.
- The system shall allow selecting an existing category by name or creating a new one if it does not exist.
- The system shall allow adding a book copy using a barcode and ISBN.
- The system shall allow viewing available books.
- The system shall allow searching books by title.
- The system shall allow searching books by author.
- The system shall allow searching books by category.
- The system shall allow marking a book copy as damaged.
- The system shall allow marking a book copy as unavailable.

### Borrowing and Returning

- The system shall allow borrowing a book copy for an active member.
- The system shall prevent borrowing if the member has unpaid fines above the allowed limit.
- The system shall prevent borrowing if the member has reached the borrowing limit for their membership type.
- The system shall prevent borrowing if the selected copy is not available.
- The system shall prevent borrowing duplicate ISBNs by the same member when restricted by business rules.
- The system shall allow returning a borrowed book copy.
- The system shall update borrowing status when a book is returned.

### Fine Management

- The system shall calculate pending fines for a member.
- The system shall display total unpaid fine amounts using currency formatting.
- The system shall allow paying a fine.
- The system shall show fine payment history for a member.

### Reports

- The system shall show books currently borrowed.
- The system shall show overdue books.
- The system shall show members with pending fines.
- The system shall show most borrowed books.
- The system shall show available books by category.
- The system shall show a member's borrowing history.

## Non-Functional Requirements

- The application should be easy to use through a numbered console menu.
- The application should follow layered architecture to separate concerns.
- The application should validate user input and show meaningful error messages.
- The application should store data reliably in PostgreSQL.
- The application should display monetary values with the ₹ currency symbol and two decimal places.
- The application should handle exceptions without crashing the entire program.
- The application should support future extension with minimal changes to existing layers.

## Data Requirements

### Core Entities

- Book
- Bookcategory
- Bookcopy
- Member
- Membershiptype
- Borrowing
- Fine
- Finepayment

### Key Data Rules

- Each member must reference a valid membership type.
- Each book must belong to a category.
- Each book copy must reference a valid ISBN.
- Each borrowing record must reference a valid member and book copy.
- Each fine must be linked to a borrowing record.
- Membership types such as Basic, Student, and Premium should be seeded in the database.

## Business Rules

- Only active members can borrow books.
- Borrowing limits depend on the membership type.
- Fine limits must be respected before new borrowing is allowed.
- A book copy can only be borrowed if it is available.
- Returned books must update borrowing status and return date.
- Damaged or unavailable copies should not be treated as available for borrowing.

## User Interface Requirements

- The application shall provide a main menu with options for member, book, borrowing, return, fine, and report operations.
- The application shall prompt users clearly for input values.
- The application shall use readable output for books, members, and transactions.
- The application shall use semantic input where possible, such as category names instead of category IDs.
- The application shall show membership type options clearly when adding a member.

## Technical Assumptions

- The application runs on .NET 8.
- PostgreSQL is available locally or on a configured server.
- Entity Framework Core is used for schema management and data access.
- Stored functions and procedures are available in the database for selected operations.
- The console application is the only user interface required for this version.

## Success Criteria

- A user can manage members, books, borrowings, fines, and reports from the console without runtime failures.
- Data is saved and retrieved correctly from PostgreSQL.
- Business rules are enforced consistently.
- The application handles empty states and invalid inputs gracefully.
- The system supports the complete end-to-end library workflow.

## Summary

This requirement analysis describes a modular console application for managing a community library. The system focuses on reliable data handling, clear user interaction, and rule-based transaction processing while keeping the design simple and maintainable.
