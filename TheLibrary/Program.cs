using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TheLibrary.Models;
using TheLibrary.UI;


namespace TheLibrary;

class Program
{
    static async Task Main(string[] args)
    { 
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json",  optional: true).Build();
        
        
        var connectionString = builder.GetConnectionString("LibraryDB");
        
        // check if connection string is not null. 
        if (string.IsNullOrEmpty(connectionString))
        {
          Console.WriteLine("Error: Connection string 'LibraryDB' not found in appsettings.json");
          return;
        }

        var options  = new DbContextOptionsBuilder<LibraryDbContext>().UseSqlServer(connectionString).Options;
        
        using var db = new LibraryDbContext(options);

        MainMenu menu = new MainMenu();

        menu.db = db;
        await menu.Run();
        
    } 
        
        /*
        
        var books = db.Books.Include(b => b.Authors).ToList();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== All available books ===");
        Console.ResetColor();
        foreach (var book in books)
        {
            var authorsNames = book.Authors.Any() ?
                string.Join(", ", book.Authors.Select(a => a.AuthorName)) : "Unknown Author";
            Console.WriteLine($"{book.BookTitle} by {authorsNames}");
        }
        */
   
}


