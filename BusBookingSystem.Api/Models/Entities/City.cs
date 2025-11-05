namespace BusBookingSystem.Api.Models.Entities
{
    public class City
    {
        public string CityId { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<BusRoute> FromRoutes { get; set; }
        public ICollection<BusRoute> ToRoutes { get; set; }
    }
}
