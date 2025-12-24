using Microsoft.EntityFrameworkCore;
using TheLibrary.Models;
using TheLibrary.UI;


namespace TheLibrary;

class Program
{
    static void Main(string[] args)
    {
        using var db = new LibraryDbContext();

        var mainMenu = new MainMenu();
        mainMenu.Run();
    }
}


