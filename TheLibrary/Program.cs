using TheLibrary.Models;

namespace TheLibrary;

class Program
{
    static void Main(string[] args)
    {
        using var db = new LibraryDbContext();
        var allActiveLoans = db.ActiveLoans.ToList();
        foreach (var loan in allActiveLoans)
        {
            Console.WriteLine(loan.LoanId);
        }
    }
}