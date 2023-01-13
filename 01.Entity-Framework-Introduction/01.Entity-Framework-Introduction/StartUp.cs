using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Collections.Generic;
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
            // Console.WriteLine(GetDepartmentsWithMoreThan5Employees(dbContext));
            // Console.WriteLine(GetLatestProjects(dbContext));
            // Console.WriteLine(IncreaseSalaries(dbContext));
            // Console.WriteLine(GetEmployeesByFirstNameStartingWithSa(dbContext));
            Console.WriteLine(DeleteProjectById(dbContext));
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
        
        //11. Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(x => x.StartDate)
                .Take(10)
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    Name = x.Name,
                    Description = x.Description,
                    StartDate = x.StartDate.ToString("M/d/yyyy h:mm:ss tt")
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var p in projects)
            {
                sb.AppendLine(p.Name);
                sb.AppendLine(p.Description);
                sb.AppendLine(p.StartDate);
            }

            return sb.ToString().TrimEnd();

        }

        //12. Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            HashSet<string> departments = new HashSet<string>()
                { "Engineering", "Tool Design", "Marketing", "Information Services" };

            var employees = context.Employees
                .Where(x => departments.Contains(x.Department.Name))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToArray();

            foreach (var emp in employees)
                emp.Salary *= 1.12m;
            
            context.SaveChanges();

            StringBuilder sb = new StringBuilder();

            foreach (var em in employees)
                sb.AppendLine($"{em.FirstName} {em.LastName} (${em.Salary:f2})");

            return sb.ToString().TrimEnd();
        }

        //13. Find Employees by First Name Starting with "Sa"
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(x => x.FirstName.StartsWith("Sa"))
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Job = x.JobTitle,
                    Salary = x.Salary
                })
                .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var em in employees)
                sb.AppendLine($"{em.FirstName} {em.LastName} - {em.Job} - (${em.Salary:f2})");

            return sb.ToString().TrimEnd();
        }
        
        //14. Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            var projectToDelete = context.Projects.First(x => x.ProjectId == 2);
            
            var empProjectsToDelete = context.EmployeesProjects
                .Where(ep => ep.ProjectId == 2)
                .ToArray();

            foreach (var empProject in empProjectsToDelete)
                context.EmployeesProjects.Remove(empProject);
            
            context.Projects.Remove(projectToDelete);
            
            context.SaveChanges();

            StringBuilder sb = new StringBuilder();

            foreach (var p in context.Projects.Take(10))
                sb.AppendLine(p.Name);

            return sb.ToString().TrimEnd();
        }

    }
}
