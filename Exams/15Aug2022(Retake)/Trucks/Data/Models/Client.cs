using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Trucks.Data.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(3, 40)]
        public string Name { get; set; }

        [Required]
        [Range(2, 40)]
        public string Nationality { get; set; }

        [Required]
        public string Type { get; set; }

        public virtual ICollection<ClientTruck> ClientsTrucks { get; set; } = new HashSet<ClientTruck>();
    }
}