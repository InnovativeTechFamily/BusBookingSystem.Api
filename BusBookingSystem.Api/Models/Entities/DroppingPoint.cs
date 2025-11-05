namespace BusBookingSystem.Api.Models.Entities
{
    public class DroppingPoint
    {
        public string PointId { get; set; }
        public string BusId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Landmark { get; set; }
        public TimeSpan Time { get; set; }
        public bool IsActive { get; set; } = true;

        public Bus Bus { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }




}
