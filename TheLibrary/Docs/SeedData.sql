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
