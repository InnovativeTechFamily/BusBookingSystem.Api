namespace BusBookingSystem.Api.Models.Entities
{
    public class BusAmenity
    {
        public string AmenityId { get; set; }
        public string BusId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation property
        public Bus Bus { get; set; }
    }

}
