namespace BusBookingSystem.Api.Models.Entities
{
    public class Booking
    {
        public string BookingId { get; set; }
        public string PNR { get; set; }
        public string UserId { get; set; }
        public string BusId { get; set; }
        public DateTime JourneyDate { get; set; }
        public string BoardingPointId { get; set; }
        public string DroppingPointId { get; set; }
        public int PassengerCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal BaseFare { get; set; }
        public decimal Taxes { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal Discount { get; set; }
        public string Status { get; set; } // PaymentPending, Confirmed, Cancelled
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; }
        public Bus Bus { get; set; }
        public BoardingPoint BoardingPoint { get; set; }
        public DroppingPoint DroppingPoint { get; set; }
        public ICollection<BookingPassenger> Passengers { get; set; }
        public Payment Payment { get; set; }
    }




}
