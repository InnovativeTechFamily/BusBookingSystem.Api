namespace BusBookingSystem.Api.Models.Entities
{
    public class BusRoute
    {
        public string RouteId { get; set; }
        public string BusId { get; set; }
        public string FromCityId { get; set; }
        public string ToCityId { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal BaseFare { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Bus Bus { get; set; }
        public City FromCity { get; set; }
        public City ToCity { get; set; }
    }




}
