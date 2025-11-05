namespace BusBookingSystem.Api.Models.Entities
{
    public class Payment
    {
        public string PaymentId { get; set; }
        public string BookingId { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; } // Pending, Success, Failed
        public string TransactionId { get; set; }
        public string UpiId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public Booking Booking { get; set; }
    }




}
