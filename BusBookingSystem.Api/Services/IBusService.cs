using BusBookingSystem.Api.Models.DTOs.Bus;
using BusBookingSystem.API.Models.DTOs.Seat;

namespace BusBookingSystem.Api.Services
{
	public interface IBusService
	{
		Task<BusSearchResponse> SearchBusesAsync(BusSearchRequest request);
		Task<SeatLayoutResponse> GetSeatLayoutAsync(string busId, DateTime journeyDate);
	}
}
