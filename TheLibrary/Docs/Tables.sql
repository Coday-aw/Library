-- 1. Create database called libraryDB
GO 
CREATE DATABASE LibraryDB
GO 
USE LibraryDB
GO 

-- 2. Create Tables 

CREATE TABLE Author (
    AuthorId INT IDENTITY(1,1) PRIMARY KEY,
    AuthorName NVARCHAR(100) NOT NULL
)

-- Book
CREATE TABLE Book (
    BookId INT IDENTITY(1,1) PRIMARY KEY,
    BookTitle NVARCHAR(100) NOT NULL,
    Genre NVARCHAR(100) NOT NULL,
    PublicationYear INT NOT NULL
);

-- Junction table for many-to-many Book-Author 
CREATE TABLE BookAuthor (
    BookId INT NOT NULL,
    AuthorId INT NOT NULL,
    CONSTRAINT PK_BookAuthor PRIMARY KEY (BookId, AuthorId),
    CONSTRAINT FK_BookAuthor_Book FOREIGN KEY (BookId) REFERENCES Book(BookId),
    CONSTRAINT FK_BookAuthor_Author FOREIGN KEY (AuthorId) REFERENCES Author(AuthorId)
);

-- Member 
CREATE TABLE Member (
    MemberId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL ,
    PhoneNumber NVARCHAR(20) UNIQUE NOT NULL
);

-- Loan 
CREATE TABLE BookLoan (
    LoanId INT IDENTITY(1,1) PRIMARY KEY,
    LoanDate DATE NOT NULL,
    FkBookId INT NOT NULL, 
    FkMemberId INT NOT NULL, 
    CONSTRAINT FK_LOAN_BOOK FOREIGN KEY(FkBookId) REFERENCES Book(BookId),
    CONSTRAINT FK_LOAN_MEMBER FOREIGN KEY(FkMemberId) REFERENCES Member(MemberId)
);

-- Return 
CREATE TABLE BookReturn (
    ReturnId INT IDENTITY(1,1) PRIMARY KEY,
    ReturnDate DATE NOT NULL,
    FkLoanId INT UNIQUE NOT NULL,
    CONSTRAINT FK_BOOKRETURN_BOOKLOAN FOREIGN KEY(FkLoanId) REFERENCES BookLoan(LoanId) 
);
