namespace BusBookingSystem.Api.Models.DTOs.Bus
{
    public class BusSearchRequest
    {
        public string FromCityId { get; set; }
        public string ToCityId { get; set; }
        public DateTime JourneyDate { get; set; }
        public int PassengerCount { get; set; }
    }
}
