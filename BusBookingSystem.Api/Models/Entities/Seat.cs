namespace BusBookingSystem.Api.Models.Entities
{
    public class Seat
    {
        public string SeatId { get; set; }
        public string BusId { get; set; }
        public string SeatNumber { get; set; }
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string Deck { get; set; }
        public string SeatType { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsLadiesOnly { get; set; }
        public bool IsWindowSeat { get; set; }
        public bool IsPremium { get; set; }
        public string PositionSide { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Bus Bus { get; set; }
        public ICollection<SeatLock> SeatLocks { get; set; }
        public ICollection<BookingPassenger> BookingPassengers { get; set; }
    }




}
