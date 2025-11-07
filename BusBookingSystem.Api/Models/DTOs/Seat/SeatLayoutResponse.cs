namespace BusBookingSystem.Api.Models.DTOs.Seat
{
	public class SeatLayoutResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public SeatLayoutData Data { get; set; }
	}

	public class SeatLayoutData
	{
		public string BusId { get; set; }
		public string BusName { get; set; }
		public DateTime JourneyDate { get; set; }
		public string LayoutType { get; set; }
		public int TotalSeats { get; set; }
		public int AvailableSeats { get; set; }
		public List<DeckDto> Decks { get; set; }
		public DateTime LastUpdated { get; set; }
	}

	public class DeckDto
	{
		public string DeckType { get; set; }
		public int TotalSeats { get; set; }
		public int AvailableSeats { get; set; }
		public List<SeatDto> Seats { get; set; }
	}

	public class SeatDto
	{
		public string SeatId { get; set; }
		public string SeatNumber { get; set; }
		public int RowNumber { get; set; }
		public int ColumnNumber { get; set; }
		public string Deck { get; set; }
		public string SeatType { get; set; }
		public decimal Price { get; set; }
		public string Status { get; set; }
		public bool IsLadiesOnly { get; set; }
		public bool IsWindowSeat { get; set; }
		public bool IsPremium { get; set; }
		public SeatPositionDto Position { get; set; }
		public string BookedBy { get; set; }
	}

	public class SeatPositionDto
	{
		public int Row { get; set; }
		public int Column { get; set; }
		public string Side { get; set; }
	}
}