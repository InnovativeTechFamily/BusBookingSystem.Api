namespace BusBookingSystem.Api.Models.DTOs.Seat
{
	public class SeatLockReleaseRequest
	{
		public string LockId { get; set; }
		public string SessionId { get; set; }
	}

	public class SeatLockReleaseResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public SeatLockReleaseData Data { get; set; }
	}

	public class SeatLockReleaseData
	{
		public string LockId { get; set; }
		public List<string> ReleasedSeats { get; set; }
		public DateTime ReleasedAt { get; set; }
	}
}