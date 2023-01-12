using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext dbContext = new SoftUniContext();

            // Console.WriteLine(GetEmployeesFullInformation(dbContext));
            // Console.WriteLine(GetEmployeesWithSalaryOver50000(dbContext));
            // Console.WriteLine(GetEmployeesFromResearchAndDevelopment(dbContext));
            // Console.WriteLine(AddNewAddressToEmployee(dbContext));
            Console.WriteLine(AddNewAddressToEmployee(dbContext));
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees.Select(x => new
            {
                x.EmployeeId,
                x.FirstName,
                x.MiddleName,
                x.LastName,
                x.JobTitle,
                x.Salary

            }).OrderBy(x => x.EmployeeId).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees.Select(x => new
            {
                x.FirstName,
                x.Salary
            })
            .Where(x => x.Salary > 50000)
            .OrderBy(x => x.FirstName)
            .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach(var em in employees)
                sb.AppendLine($"{em.FirstName} - {em.Salary:f2}");

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(x => x.Department.Name == "Research and Development")
                .Select( x => new
                {
                    x.FirstName,x.LastName,x.Department,x.Salary
                })
                .OrderBy(x => x.Salary).ThenByDescending(x => x.FirstName).ToArray();

            StringBuilder sb = new StringBuilder();

            foreach(var em in employees)
                 sb.AppendLine($"{em.FirstName} {em.LastName} from {em.Department.Name} - ${em.Salary:f2}");
            
            return sb.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address newAddress = new Address() { AddressText = "Vitoshka 15", TownId = 4 };
            context.Addresses.Add(newAddress);

            var nakov = context.Employees.First(x => x.LastName == "Nakov");
            nakov.Address = newAddress;

            context.SaveChangesAsync();

            var addressText = context.Employees.OrderByDescending(x => x.AddressId).Take(10)
                .Select(x => x.Address.AddressText).ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var ad in addressText) sb.AppendLine(ad);

            return sb.ToString().TrimEnd();
        }
    }
}
