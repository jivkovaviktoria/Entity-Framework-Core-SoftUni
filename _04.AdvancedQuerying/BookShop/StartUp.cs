using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BookShop.Models;
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
            
            // Console.WriteLine(GetBooksByPrice(db));

            // var year = int.Parse(Console.ReadLine());
            // Console.WriteLine(GetBooksNotReleasedIn(db, year));

            //var input = Console.ReadLine();
            //Console.WriteLine(GetBooksByCategory(db, input));

            var date = Console.ReadLine();
            Console.WriteLine(GetBooksReleasedBefore(db, date));
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

        // 5. Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
                sb.AppendLine(b);

            return sb.ToString().TrimEnd();
        }
        
        // 6. Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            HashSet<string> categories = new HashSet<string>(input.Split(), StringComparer.OrdinalIgnoreCase);

            var books = context.Books
                .Where(b => b.BookCategories.Any(bc => categories.Contains(bc.Category.Name)))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
                sb.AppendLine(b);

            return sb.ToString().TrimEnd();
        }
        
        // 7. Released before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var targetDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value < targetDate)
                .Select(b => new
                {
                    Title = b.Title,
                    Edinition = b.EditionType,
                    Price = b.Price,
                    ReleaseDate = b.ReleaseDate
                })
                .OrderByDescending(b => b.ReleaseDate)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
                sb.AppendLine($"{b.Title} - {b.Edinition} - ${b.Price:f2}");

            return sb.ToString().TrimEnd();
        }
    }
}
