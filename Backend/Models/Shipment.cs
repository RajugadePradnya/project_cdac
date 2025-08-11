using System;

namespace RapidReachApi.Models
{
    public class Shipment
    {
        public int ShipmentId { get; set; }

        public string SenderAddress { get; set; }
        public string ReceiverAddress { get; set; }
        public double ParcelWeightKg { get; set; }
        public string ServiceType { get; set; }
        public string DeliveryBranch { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
