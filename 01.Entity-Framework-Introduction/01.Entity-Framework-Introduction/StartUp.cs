using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
            // Console.WriteLine(AddNewAddressToEmployee(dbContext));
            // Console.WriteLine(GetEmployeesInPeriod(dbContext));
            // Console.WriteLine(GetAddressesByTown(dbContext));
            // Console.WriteLine(GetEmployee147(dbContext));
            Console.WriteLine(GetDepartmentsWithMoreThan5Employees(dbContext));
        }

        //03. Employees full information
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

        //04. Employees with salary over 50 000
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

        //05. Employees from research and development
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

        //06. Adding a new address and updating employee
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

        //07. Employees and projects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(x => x.EmployeesProjects.Any(p =>
                    p.Project.StartDate.Year >= 2001 && p.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(x => new { x.FirstName, x.LastName, x.Manager, Projects = x.EmployeesProjects.Select(p => new
                {
                    p.Project.Name,
                    StartDate = p.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                    EndDate = p.Project.EndDate.HasValue ? p.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished"
                })})
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var em in employees)
            {
                sb.AppendLine($"{em.FirstName} {em.LastName} - Manager: {em.Manager.FirstName} {em.Manager.LastName}");
                foreach (var p in em.Projects)
                    sb.AppendLine($"--{p.Name} - {p.StartDate} - {p.EndDate}");
            }

            return sb.ToString().TrimEnd();
        }

        //08. Addresses by town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                .Select(x => new { Address = x.AddressText, Town = x.Town.Name, Employees = x.Employees.Count })
                .OrderByDescending(x => x.Employees).ThenBy(x => x.Town).ThenBy(x => x.Address)
                .Take(10).ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var ad in addresses)
                sb.AppendLine($"{ad.Address}, {ad.Town} - {ad.Employees} employees");

            return sb.ToString().TrimEnd();
        }

        //09. Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees.Include(e => e.EmployeesProjects).ThenInclude(ep => ep.Project)
                .FirstOrDefault(e => e.EmployeeId == 147);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            foreach (var ep in employee.EmployeesProjects.OrderBy(x => x.Project.Name))
                sb.AppendLine(ep.Project.Name);
            
            return sb.ToString().TrimEnd();
        }

        //10. Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(x => x.Employees.Count > 5)
                .Select(x => new
                {
                    Name = x.Name,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    Employees = x.Employees
                })
                .OrderBy(x => x.Employees.Count).ThenBy(x => x.Name)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var dep in departments)
            {
                sb.AppendLine($"{dep.Name} - {dep.ManagerFirstName} {dep.ManagerLastName}");

                foreach (var em in dep.Employees.OrderBy(x => x.FirstName).ThenBy(x => x.LastName))
                    sb.AppendLine($"{em.FirstName} {em.LastName} - {em.JobTitle}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
