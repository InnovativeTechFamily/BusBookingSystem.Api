namespace BusBookingSystem.Api.Models.Entities
{
    public class Bus
    {
        public string BusId { get; set; }
        public string BusName { get; set; }
        public string BusNumber { get; set; }
        public string BusType { get; set; }
        public string OperatorId { get; set; }
        public int TotalSeats { get; set; }
        public string LayoutType { get; set; }
        public bool IsPremium { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public BusOperator Operator { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public ICollection<BusRoute> Routes { get; set; }
        public ICollection<BoardingPoint> BoardingPoints { get; set; }
        public ICollection<DroppingPoint> DroppingPoints { get; set; }
        public ICollection<BusAmenity> Amenities { get; set; }
    }

}
