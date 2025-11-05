namespace BusBookingSystem.Api.Models.DTOs.Bus
{
    public class BusSearchResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public BusSearchData Data { get; set; }
    }

    public class BusSearchData
    {
        public string SearchId { get; set; }
        public int TotalBuses { get; set; }
        public List<BusDto> Buses { get; set; }
    }

    public class BusDto
    {
        public string BusId { get; set; }
        public string BusName { get; set; }
        public string BusNumber { get; set; }
        public string BusType { get; set; }
        public string OperatorId { get; set; }
        public string OperatorName { get; set; }
        public decimal Rating { get; set; }
        public int TotalReviews { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string DepartureLocation { get; set; }
        public string ArrivalLocation { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public decimal BaseFare { get; set; }
        public bool IsPremium { get; set; }
        public List<string> Amenities { get; set; }
        public string CancellationPolicy { get; set; }
        public List<BoardingPointDto> BoardingPoints { get; set; }
        public List<DroppingPointDto> DroppingPoints { get; set; }
    }

    public class BoardingPointDto
    {
        public string PointId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Time { get; set; }
        public string Landmark { get; set; }
    }

    public class DroppingPointDto
    {
        public string PointId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Time { get; set; }
        public string Landmark { get; set; }
    }
}
