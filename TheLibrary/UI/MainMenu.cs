using Microsoft.EntityFrameworkCore;
using TheLibrary.Models;
namespace TheLibrary.UI;

public class MainMenu
{
   public LibraryDbContext? db  { get; set; }
    public void Run()
    {
        while (true)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===================================");
            Console.WriteLine("         📚 LIBRARY SYSTEM         ");
            Console.WriteLine("===================================");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("1. Admin Menu");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("2. Member Menu");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0. Exit");
            Console.ResetColor();

            Console.Write("\nSelect option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AdminMenu();
                    break;
                case "2":
                    MemberMenu();
                    break;
                case "0":
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("\nGoodbye! 👋");
                    Console.ResetColor();
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Press any key...");
                    Console.ResetColor();
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
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("=== 👨‍💼 Admin Menu ===");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("1. Add Book");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("2. Delete Book");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("3. Edit Book (Update StockBalance)");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0. Back");
            Console.ResetColor();

            Console.Write("\nSelect option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddBook();
                    break;
                case "2":
                   await DeleteBook();
                    break;
                case "3":
                    Console.WriteLine("👉 Call UpdateBook() here");
                    break;
                case "0":
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Press any key...");
                    Console.ResetColor();
                    Console.ReadKey();
                    break;
            }
        } 
    }
    public void MemberMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== 👤 Member Menu ===");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("1. Loan Book");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("2. Return Book");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("3. Show All Books");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("4. Search for Books");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0. Back");
            Console.ResetColor();

            Console.Write("\nSelect option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // Call your LoanBook() method
                    Console.WriteLine("👉 Call LoanBook() here");
                    break;
                case "2":
                    // Call your ReturnBook() method
                    Console.WriteLine("👉 Call ReturnBook() here");
                    break;
                case "3":
                    // Call your ShowAllBooks() method
                    Console.WriteLine("👉 Call ShowAllBooks() here");
                    break;
                case "4":
                    Console.Write("Enter search term: ");
                    string searchTerm = Console.ReadLine();
                    // Call your SearchBooks(searchTerm) method
                    Console.WriteLine($"👉 Call SearchBooks(\"{searchTerm}\") here");
                    break;
                case "0":
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Press any key...");
                    Console.ResetColor();
                    Console.ReadKey();
                    break;
            }
        }   
    }

    public void AddBook()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Enter book title: ");
        Console.Write(">");
        string title = Console.ReadLine();
        Console.WriteLine("Enter genre: ");
        Console.Write(">");
        string genre = Console.ReadLine();
        Console.WriteLine("Enter author Name: ");
        Console.Write(">");
        string author = Console.ReadLine();
        Console.WriteLine("Publication Year: ");
        Console.Write(">");
        int publicationYearOfTheBook;
        int.TryParse(Console.ReadLine(), out publicationYearOfTheBook);
        Console.ResetColor();
        try
        {
            var newAuthor = new Author{AuthorName = author};
            var newBook = new Book { BookTitle = title, Genre = genre, PublicationYear = publicationYearOfTheBook };
            
            newAuthor.Books.Add(newBook);
            db.Authors.Add(newAuthor);
            db.SaveChanges();
            Console.WriteLine($"Added new book: {newBook.BookTitle} - {newAuthor.AuthorName}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private async Task DeleteBook()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Enter book ID: ");
        Console.Write(">");
        int bookID;

        if (int.TryParse(Console.ReadLine(), out bookID))
        {
            await db.DeleteBookAsync(bookID);
            Console.WriteLine($"Deleted book: {bookID}");
        }
        else
        {
            Console.WriteLine("Invalid ID");
        }
    }
}