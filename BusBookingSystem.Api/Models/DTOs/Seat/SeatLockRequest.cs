using BusBookingSystem.Api.Models.DTOs.Bus;

namespace BusBookingSystem.Api.Models.DTOs.Seat
{
	public class SeatLockRequest
	{
		public string BusId { get; set; }
		public DateTime JourneyDate { get; set; }
		public List<string> SeatIds { get; set; }
		public string BoardingPointId { get; set; }
		public string DroppingPointId { get; set; }
	}

	public class SeatLockResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public SeatLockData Data { get; set; }
		public List<SeatLockError> Errors { get; set; }
	}

	public class SeatLockData
	{
		public string LockId { get; set; }
		public string SessionId { get; set; }
		public string BusId { get; set; }
		public DateTime JourneyDate { get; set; }
		public List<LockedSeatDto> LockedSeats { get; set; }
		public BoardingPointDto BoardingPoint { get; set; }
		public DroppingPointDto DroppingPoint { get; set; }
		public DateTime LockExpiresAt { get; set; }
		public DateTime NewExpiryTime { get; set; }
		public int LockDurationSeconds { get; set; }
		public int RemainingSeconds { get; set; }
		public PriceBreakdownDto PriceBreakdown { get; set; }
	}

	public class LockedSeatDto
	{
		public string SeatId { get; set; }
		public string SeatNumber { get; set; }
		public decimal Price { get; set; }
	}

	public class PriceBreakdownDto
	{
		public decimal BaseFare { get; set; }
		public decimal Taxes { get; set; }
		public decimal ServiceFee { get; set; }
		public decimal Discount { get; set; }
		public decimal TotalAmount { get; set; }
	}

	public class SeatLockError
	{
		public string SeatId { get; set; }
		public string SeatNumber { get; set; }
		public string Status { get; set; }
		public DateTime? LockedUntil { get; set; }
	}
}