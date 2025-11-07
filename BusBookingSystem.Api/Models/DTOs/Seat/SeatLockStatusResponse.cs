namespace BusBookingSystem.Api.Models.DTOs.Seat
{
	public class SeatLockStatusResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public SeatLockStatusData Data { get; set; }
	}

	public class SeatLockStatusData
	{
		public string LockId { get; set; }
		public string Status { get; set; }
		public List<string> LockedSeats { get; set; }
		public DateTime ExpiresAt { get; set; }
		public int RemainingSeconds { get; set; }
		public bool CanExtend { get; set; }
	}
}