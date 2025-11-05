namespace BusBookingSystem.Api.Models.Entities
{
    public class SeatLock
    {
        public string LockId { get; set; }
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public string BusId { get; set; }
        public DateTime JourneyDate { get; set; }
        public string SeatId { get; set; }
        public string BoardingPointId { get; set; }
        public string DroppingPointId { get; set; }
        public DateTime LockedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Status { get; set; } // Active, Released, Expired

        // Navigation properties
        public ApplicationUser User { get; set; }
        public Bus Bus { get; set; }
        public Seat Seat { get; set; }
        public BoardingPoint BoardingPoint { get; set; }
        public DroppingPoint DroppingPoint { get; set; }
    }




}
