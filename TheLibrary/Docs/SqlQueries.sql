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


--Add member
INSERT INTO Member( FirstName, LastName, Email, PhoneNumber) VALUES 
 ('Coday', 'Awahmed', 'Coday@gmail.com', '0700727429');
INSERT INTO Member( FirstName, LastName, Email, PhoneNumber) VALUES 
 ('Peter', 'Parker', 'peter@gmail.com', '0700727430');
INSERT INTO Member( FirstName, LastName, Email, PhoneNumber) VALUES 
 ('Steve', 'Rogers', 'steve@gmail.com', '0700727431');
INSERT INTO Member( FirstName, LastName, Email, PhoneNumber) VALUES 
 ('Tony', 'Stark', 'tony@gmail.com', '0700727432');

-- Add Authors
 INSERT INTO Author (AuthorName) VALUES
( 'J.K Rowling'),
( 'J.R.R. Tolkien'),
('George Orwell'),
('Harper Lee');

--Add  Books
INSERT INTO Book( BookTitle,Genre,PublicationYear) VALUES
('Harry Potter and the Philosopher Stone', 'Fantasy', 1997),
('The Hobbit', 'Fantasy', 1937),
('1984', 'Dystopian', 1949),
('To Kill a Mockingbird', 'Fiction', 1960);

-- Add books and authors into BookAuthor (junction table)
INSERT INTO BookAuthor(BookId, AuthorId) VALUES
(1,1),
(2, 2),
(3, 3), 
(4, 4);

-- Loan a book
INSERT INTO BookLoan(LoanDate, FkBookId, FkMemberId) VALUES
('2025-11-29', 1, 1 )

-- Return a book
INSERT INTO BookReturn (ReturnDate, FkLoanId) VALUES
('2025-11-29', 1)

-- Create a view for active loans for books
CREATE VIEW ActiveLoans AS 
SELECT
l.LoanId,
L.LoanDate,
b.BookTitle,
b.Genre,
b.PublicationYear,
a.AuthorName,
m.MemberId, 
m.FirstName,
M.LastName
FROM BookLoan l 
JOIN Book b 
ON l.FkBookId = b.BookId 
JOIN Member m 
ON l.FkMemberId = m.MemberId
JOIN BookAuthor ba 
ON b.BookId = ba.BookId 
JOIN Author a 
ON ba.AuthorId = a.AuthorId
WHERE NOT EXISTS (
    SELECT 1 FROM BookReturn br WHERE br.FkLoanId = l.LoanId -- this is to make sure the book is not returned
)

-- Get all active loans 
SELECT * FROM ActiveLoans;

-- Create procedure for loans 
ALTER PROCEDURE RegisterLoan 
        @LoanDate DATE, 
        @FkBookId INT , 
        @FkMemberId INT
AS 
BEGIN

-- check if book member
IF NOT EXISTS (
    SELECT 1 
    FROM Member 
    WHERE MemberId = @FkMemberId
)
-- check if book exist
OR NOT EXISTS (
    SELECT 1
    FROM Book 
    WHERE BookId = @FkBookId
)
-- if not throw error and return
BEGIN 
RAISERROR ('Book or member does not exists, make sure both book and member exist', 16, 1)
RETURN;
END;
-- else create a loan
    INSERT INTO BookLoan(LoanDate,FkBookId,FkMemberId)
    VALUES ( @LoanDate,@FkBookId, @FkMemberId )
END;

-- procedure to delete a book. and i also delete from the bookAuthor table (junction table)
CREATE PROCEDURE DeleteBook 
      @BookId INT
AS 
BEGIN
IF NOT EXISTS(
SELECT 1 
FROM Book 
WHERE BookId = @BookId
)
BEGIN
RAISERROR('Cannot delete, the book does not exist', 16, 1);
RETURN;
END;
BEGIN TRANSACTION
  DELETE FROM BookAuthor 
  WHERE BookId = @BookId

  DELETE FROM Book
  WHERE BookId = @BookId
COMMIT;
END; 


-- Register a new loan for member 1 
EXEC RegisterLoan  @LoanDate = '2025-11-30', @FkBookId = 4, @FkMemberId = 1;

-- Did not have a stock column, so ia added a stock balance to books so can kan create a trigger that updates stocks
ALTER TABLE Book ADD StockBalance INT DEFAULT 1 NOT NULL;


-- this trigger will update stock balance for when books are returned
CREATE TRIGGER trg_stockBalanceReturnUpdate
ON BookReturn 
AFTER INSERT 
AS 
BEGIN
UPDATE b 
SET b.StockBalance = b.StockBalance + 1
FROM Book b
INNER JOIN BookLoan bl ON b.BookId = bl.FkBookId
INNER JOIN inserted i ON bl.LoanId = i.FkLoanId
PRINT 'Stock balance update for borrowed book'
END;

-- this trigger will update stock balance for then books are borrowed and prevent loans when stockBalance is 0
CREATE TRIGGER trg_stockBalanceLoanUpdate
ON BookLoan
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- First check stock and prevent if stockBalance is 0
    IF EXISTS (
        SELECT 1
        FROM Book b
        INNER JOIN inserted i ON b.BookId = i.FkBookId
        WHERE b.StockBalance <= 0
    )
    BEGIN
        RAISERROR ('Cannot borrow, book is out of stock', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END;

    -- update stockBalance
    UPDATE b
    SET b.StockBalance = b.StockBalance - 1
    FROM Book b
    INNER JOIN inserted i ON b.BookId = i.FkBookId;
END;

-- isolation level serializable makes sure no other transaction 
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE 
BEGIN TRANSACTION

UPDATE Book 
WHERE BookId = @FkBookId AND StockBalance > 0;

EXEC RegisterLoan '2020-11-17', 1054, 4;

COMMIT;

-- Read committed data only. if any transaction is going on the select will wait
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
BEGIN TRANSACTION

SELECT * FROM ActiveLoans;

COMMIT;

-- read committed makes sure not you do not read a stock balance data that is being changed
SET TRANSACTION ISOLATION LEVEL REDAD COMMITTED 
BEGIN TRANSACTION

-- check if the book is in stock
SELECT StockBalance FROM Book WHERE BookId = @FkBookId;
-- and then vi register loan
INSERT INTO BookLoan(LoanDate, FkBookId, FkMemberId) 
VALUES (@LoanDate, @FkBookId, @FkMemberId) 

COMMIT;


-- search by title without index 
SELECT BookTitle FROM Book WHERE BookTitle LIKE 'The great%' -- reslut: clustered index scan. all 55 rows read 


-- create index for book search by title 
CREATE INDEX IX_Book_Title 
ON Book(BookTitle);

-- same query after index 
SELECT BookTitle FROM Book WHERE BookTitle LIKE 'The great%' -- result: Seek Index. 1 row read 



















