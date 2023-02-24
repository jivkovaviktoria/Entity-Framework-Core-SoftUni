using System.ComponentModel.DataAnnotations.Schema;

namespace Trucks.Data.Models
{
    public class ClientTruck
    {
        public int ClientId { get; set; }
        
        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; }

        public int TruckId { get; set; }
        
        [ForeignKey(nameof(TruckId))]
        public Truck Truck { get; set; }
    }
}