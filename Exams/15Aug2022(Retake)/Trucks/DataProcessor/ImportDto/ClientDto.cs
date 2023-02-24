using System.Collections.Generic;
using Trucks.Data.Models;

namespace Trucks.DataProcessor.ImportDto
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }
        public string Type { get; set; }
        public List<int> Trucks { get; set; }
    }
}