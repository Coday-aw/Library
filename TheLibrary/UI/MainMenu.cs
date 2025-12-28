using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TheLibrary.Models;

namespace TheLibrary.UI;

public class MainMenu
{
    public LibraryDbContext db { get; set; } 

    public async Task Run() 
    {
        while (true)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===================================");
            Console.WriteLine("         📚 LIBRARY SYSTEM         ");
            Console.WriteLine("===================================");
            Console.ResetColor();

            Console.WriteLine("1. Admin Menu");
            Console.WriteLine("2. Member Menu");
            Console.WriteLine("0. Exit");

            Console.Write("\nSelect option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await AdminMenu(); // Added await
                    break;
                case "2":
                    await MemberMenu(); // Added await
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Press any key...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    public async Task AdminMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("======================================================================");
            Console.WriteLine("                                 Admin Menu                           ");
            Console.WriteLine("======================================================================");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("1. Add Book\n2. Create Member\n3. Delete Book\n4. View active loans \n0. Back");
            Console.ResetColor();

            string choice = Console.ReadLine();
            if (choice == "0") break;

            switch (choice)
            {
                case "1":
                    AddBook();
                    break;
                case "2":
                    CreateMember();
                    break;
                case "3":
                     await DeleteBook();
                    break;
                case "4":
                    ViewActiveLoans();
                    break;
            }
            Console.WriteLine("\nPress any key to return to Admin Menu...");
            Console.ReadKey();
        }
    }

    public async Task MemberMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("======================================================================");
            Console.WriteLine("                           Member Menu                                ");
            Console.WriteLine("======================================================================");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("1. Show All Books \n2. Loan Book  \n3. Return Book \n4. Search for book \n0. Back");
            Console.ResetColor();

            string choice = Console.ReadLine();
            if (choice == "0") break;

            switch (choice)
            {
                case "1":
                    ShowAllBooks();
                    break;
                case "2":
                    await LoanBook();
                    break;
                case "3":
                    ReturnBook();
                    break;
                case "4":
                    BookSearch();
                    break;
                    
            }
            Console.WriteLine("\nPress any key to return to Member Menu...");
            Console.ReadKey();
        }
    }

    public void BookSearch()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("===============================================");
        Console.WriteLine("               SEARCH CATALOG                  ");
        Console.WriteLine("===============================================");
        Console.ResetColor();

        Console.Write("Enter Book Title: ");
        string name = Console.ReadLine();

        // Search 
        var book = db.Books
            .Include(b => b.Authors)
            .FirstOrDefault(b => b.BookTitle.Contains(name));

        if (book != null)
        {
            var authorNames = book.Authors.Any() 
                ? string.Join(", ", book.Authors.Select(a => a.AuthorName)) 
                : "Unknown Author";

            //  Display Results in a nice format
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("BOOK FOUND");
            Console.ResetColor();
            Console.WriteLine($"{"Title:",-15} {book.BookTitle}");
            Console.WriteLine($"{"Author(s):",-15} {authorNames}");
            Console.WriteLine($"{"Genre:",-15} {book.Genre}");
            Console.WriteLine($"{"Published:",-15} {book.PublicationYear}");
            
            // different colors for in stock and out of stock
            Console.Write($"{"Availability:",-15} ");
            if (book.StockBalance > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{book.StockBalance} in stock");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Out of Stock");
            }
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: No book found with that title.");
            Console.ResetColor();
        }

        Console.WriteLine("\nPress any key to return...");
        Console.ReadKey();
    }

    public void ReturnBook()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("======================================================================");
        Console.WriteLine("                               Add New Book                           ");
        Console.WriteLine("======================================================================");
        Console.WriteLine("Enter Loan ID: ");
        Console.Write("> ");


        if (!int.TryParse(Console.ReadLine(), out int loanID))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: Invalid ID");
            Console.ResetColor();
            Console.ReadKey();
            return;
        }
        
        // check if the loan exist 
        var findLoan = db.BookLoans.FirstOrDefault(b => b.LoanId == loanID);
        if (findLoan == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: No loan found with that ID.");
            Console.ResetColor();
            Console.ReadKey();
            return;
        }
        // check if the loan is already returned 
        bool alreadyReturned = db.BookLoans.Any(b => b.LoanId == loanID);
        if (alreadyReturned)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Loan already returned.");
            Console.ResetColor();
            Console.ReadKey();
            return;
        }
        // if create the return
        var newReturn = new BookReturn
        {
            ReturnDate = DateOnly.FromDateTime(DateTime.Now),
            FkLoanId = loanID,
        };
        db.BookReturns.Add(newReturn);
        db.SaveChanges();
        Console.WriteLine("Loan return successful!");
        Console.ResetColor();
    }


    private async Task LoanBook()
    {
        Console.Clear();
        Console.WriteLine("Enter book Name: ");
        string bookName = Console.ReadLine()?.ToLower();
        Console.WriteLine("Enter Your ID number: ");
        int.TryParse(Console.ReadLine(), out int memberInput);

        // NULL CHECKS: Prevent crashing if not found
        var member = await db.Members.FirstOrDefaultAsync(m => m.MemberId == memberInput);
        var book = await db.Books.FirstOrDefaultAsync(b => b.BookTitle.ToLower() == bookName);

        if (member == null || book == null)
        {
            Console.WriteLine("Error: Member or Book not found.");
            return;
        }
        
        string memberName = $"{member.FirstName}  {member.LastName}";

        await db.RegisterLoanAsync(loanDate: DateTime.Today, bookId: book.BookId, memberId: member.MemberId);
        Console.WriteLine($"Success! {book.BookTitle} loaned to {memberName}.");
    }

    public void ShowAllBooks()
    {
        Console.Clear();
        var books = db.Books.Include(b => b.Authors).ToList();

        // Define column widths
        string titleWidth = "{0,-35}";
        string authorWidth = "{1,-25}";

        // Print Header
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n{0,-35} {1,-25} {2,-12}", "Book Title", "Author", "Availability");
        Console.WriteLine(new string('-', 75));
        Console.ResetColor();
        
        foreach (var b in books)
        {
            var authorNames = b.Authors.Any() 
                ? string.Join(", ", b.Authors.Select(a => a.AuthorName)) 
                : "Unknown Author";
            
            string displayTitle = b.BookTitle.Length > 32 ? b.BookTitle.Substring(0, 31) + "…" : b.BookTitle;
            string displayAuthor = authorNames.Length > 22 ? authorNames.Substring(0, 21) + "…" : authorNames;

            
            Console.Write(string.Format("{0,-35} ", displayTitle));
            Console.Write(string.Format("{0,-25} ", displayAuthor));

            if (b.StockBalance > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{b.StockBalance} in stock");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Out of Stock");
            }
    
            Console.ResetColor();
        }
    }
    

    public void AddBook()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("======================================================================");
        Console.WriteLine("                                  Add New Book                        ");
        Console.WriteLine("======================================================================");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Enter title: ");
        Console.Write("> ");
        string title = Console.ReadLine();
        Console.WriteLine("Enter book genre: ");
        Console.Write("> ");
        string genre = Console.ReadLine();
        Console.WriteLine("Enter publication year: ");
        Console.Write("> ");
        int.TryParse(Console.ReadLine(), out int publicationYear);
        Console.WriteLine("Enter stock balance: ");
        Console.Write("> ");
        int.TryParse(Console.ReadLine(), out int stockBalance);
        Console.WriteLine("Enter Author(s) Name: ");
        Console.Write("> ");
        string authorName = Console.ReadLine();
        Console.ResetColor();

        if (title.IsNullOrEmpty() || genre.IsNullOrEmpty() || publicationYear < 1000 ||
            publicationYear > DateTime.Now.Year || stockBalance < 1)
        {
            Console.WriteLine("Error: invalid inputs.");
            return;
        }

        // check if the author already exist in the DB
        var existintAuthor = db.Authors.FirstOrDefault(a => a.AuthorName == authorName);
     
        // create new book, if author already exist link the author to this book else create new author
        var book = new Book
            { BookTitle = title, Genre = genre, PublicationYear = publicationYear, StockBalance = stockBalance };
        if (existintAuthor != null)
        {
           book.Authors.Add(existintAuthor);
        }
        else
        {
            var newAuthor = new Author { AuthorName = authorName };
            book.Authors.Add(newAuthor);
        }

        // add book to DB and handle error
        try
        {
            db.Books.Add(book);
            db.SaveChanges();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{title} has successfully been added to the library!");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error while adding book: " + ex.Message);
        }
        Console.ResetColor();
        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
    }


    public void CreateMember()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("======================================================================");
        Console.WriteLine("                                Create New Member                     ");
        Console.WriteLine("======================================================================");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Enter your first name: ");
        Console.Write("> ");
        string firstName = Console.ReadLine();
        Console.WriteLine("Enter your last name: ");
        Console.Write("> ");
        string lastName = Console.ReadLine();
        Console.WriteLine("Enter your email: ");
        Console.Write("> ");
        string email = Console.ReadLine();
        Console.WriteLine("Enter your phone number: ");
        Console.Write("> ");
        string phone = Console.ReadLine();
        Console.ResetColor();
        

        if (firstName.IsNullOrEmpty() || lastName.IsNullOrEmpty() || email.IsNullOrEmpty() || phone.IsNullOrEmpty())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("invalid inputs, please try again"); 
            Console.ResetColor();
            return;
        }

        try
        {
            var newMember = new Member { FirstName = firstName, LastName = lastName, Email = email,  PhoneNumber = phone };
            db.Members.Add(newMember);
            db.SaveChanges();
            Console.WriteLine($"Member with ID: {newMember.MemberId} added!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while creating member: " + ex.Message);
        }
        Console.ResetColor();
        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
       
    }
    

    private async Task DeleteBook()
    {
        Console.Write("Enter ID to delete: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            await db.DeleteBookAsync(id);
            Console.WriteLine("Book deleted.");
        }
    }

   public void ViewActiveLoans()
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("==========================================================================================");
    Console.WriteLine("                                  CURRENT ACTIVE LOANS                                  ");
    Console.WriteLine("==========================================================================================");
    Console.ResetColor();

    var activeLoans = db.ActiveLoans.ToList();

    if (!activeLoans.Any())
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n            There are currently no active loans in the system.            ");
        Console.ResetColor();
    }
    else
    {
        // Define Column Widths: ID(5), Date(12), Book(25), Member(20)
        string headerFormat = "{0,-5} {1,-12} {2,-25} {3,-20}";
        
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(headerFormat, "ID", "Date", "Book Title", "Borrowed By");
        Console.WriteLine(new string('-', 90));
        Console.ResetColor();

        foreach (var loan in activeLoans)
        {
            // Format the date nicely (yyyy-MM-dd)
            string displayDate = loan.LoanDate.ToString("yyyy-MM-dd");
            
            // Combine names
            string fullName = $"{loan.FirstName} {loan.LastName}";

            // Truncate long strings to keep the table stable
            string bookTitle = loan.BookTitle.Length > 23 ? loan.BookTitle.Substring(0, 21) + ".." : loan.BookTitle;
            string memberName = fullName.Length > 18 ? fullName.Substring(0, 16) + ".." : fullName;

            Console.WriteLine(headerFormat, 
                loan.LoanId, 
                displayDate, 
                bookTitle, 
                memberName);
        }
    }

    Console.WriteLine(new string('=', 90));
    Console.WriteLine("\nPress any key to return to menu...");
    Console.ReadKey();
}
}