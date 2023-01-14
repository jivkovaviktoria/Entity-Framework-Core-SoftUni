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

            var command = Console.ReadLine();
            Console.WriteLine(GetBooksByAgeRestriction(db, command));
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
    }
}
