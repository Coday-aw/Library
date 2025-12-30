## Library Project

## Features
- Book Management: Add, Update, and Delete books from the catalog.
- Register Loans: Check out books with automatic validation.
- Double-Loan Prevention: Logic to ensure a book cannot be borrowed if it is not in stock.
- Existence Validation: Validates that both the Member and Book exist before creating a loan.

## Database Optimization
- Stored Procedures: All core logic is handled on the database level for security and speed.
- SQL Indexing: Indexed BookTitle for fast searching.
- Indexed FkMemberId for quick loan history retrieval.

## Tech Stack
- Language: C# (.NET 8.0)
- Database: SQL Server
- ORM: Entity Framework Core

## How to Run
- Open SQL Server Management Studio (SSMS).
- Run the provided database schema script to create the Book, Member, BookAuthor, and BookLoan tables.
- Execute the Stored Procedures (DeleteBook, RegisterLoan, etc.) found in the /SQL folder of this project.

## Entity Relationship Diagram
<img width="3399" height="2496" alt="erdplus (11)" src="https://github.com/user-attachments/assets/c53bf507-2763-40e8-9535-6f23c32c3869" />


## Video Deno
https://github.com/user-attachments/assets/37b9380b-7cf1-4318-b20f-bdeb322a4eef







