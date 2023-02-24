using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Footballers.Data.Models
{
    public class Coach
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(2,40)]
        public string Name { get; set; }

        [Required]
        public string Nationality { get; set; }

        public virtual ICollection<Footballer> Footballers { get; set; } = new HashSet<Footballer>();
    }
}