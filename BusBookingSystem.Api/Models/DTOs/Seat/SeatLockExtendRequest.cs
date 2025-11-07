namespace BusBookingSystem.Api.Models.DTOs.Seat
{
	public class SeatLockExtendRequest
	{
		public string LockId { get; set; }
		public string SessionId { get; set; }
	}

	public class SeatLockExtendResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public SeatLockExtendData Data { get; set; }
	}

	public class SeatLockExtendData
	{
		public string LockId { get; set; }
		public DateTime NewExpiryTime { get; set; }
		public int RemainingSeconds { get; set; }
	}
}