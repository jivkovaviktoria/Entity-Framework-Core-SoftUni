using System;
using System.Linq;
using System.Text;
using BookShop.Models.Enums;

namespace BookShop
{
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            // var command = Console.ReadLine();
            // Console.WriteLine(GetBooksByAgeRestriction(db, command));
            // Console.WriteLine(GetGoldenBooks(db));
            Console.WriteLine(GetBooksByPrice(db));
        }
        
        // 2. Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var cmd = Enum.Parse<AgeRestriction>(command, true);

            var books = context.Books
                .Where(b => b.AgeRestriction == cmd)
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
                sb.AppendLine(b);

            return sb.ToString().TrimEnd();
        }

        // 3. Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            var type = Enum.Parse<EditionType>("Gold");

            var books = context.Books
                .Where(b => b.EditionType == type && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
                sb.AppendLine(b);

            return sb.ToString().TrimEnd();
        }

        // 4. Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new { Title = b.Title, Price = b.Price })
                .OrderByDescending(b => b.Price)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
                sb.AppendLine($"{b.Title} - ${b.Price:f2}");

            return sb.ToString().TrimEnd();
        }
    }
}
