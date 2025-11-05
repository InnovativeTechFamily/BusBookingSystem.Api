namespace BusBookingSystem.Api.Models.Entities
{
    public class BookingPassenger
    {
        public string PassengerId { get; set; }
        public string BookingId { get; set; }
        public string SeatId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string IdProofType { get; set; }
        public string IdProofNumber { get; set; }

        // Navigation properties
        public Booking Booking { get; set; }
        public Seat Seat { get; set; }
    }




}
