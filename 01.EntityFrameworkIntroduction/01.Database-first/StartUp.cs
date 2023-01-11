using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;

namespace SoftUni
{
    internal class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext dbContext = new SoftUniContext();
            string result = GetEmployeesFullInformation(dbContext);
            Console.WriteLine(result);
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Employee[] employees = context.Employees.OrderBy(x => x.EmployeeId).ToArray();
            foreach (var em in employees)
                sb.AppendLine($"{em.FirstName} {em.MiddleName} {em.LastName} {em.JobTitle} {em.Salary:f2}");

            return sb.ToString().TrimEnd();

        }
    }
}
