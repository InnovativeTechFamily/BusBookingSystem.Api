using BusBookingSystem.Api.Models.DTOs.Seat;

namespace BusBookingSystem.Api.Interfaces
{
	public interface ISeatService
	{
		Task<SeatLockResponse> LockSeatsAsync(SeatLockRequest request, string userId);
		Task<SeatLockResponse> ExtendSeatLockAsync(SeatLockExtendRequest request, string userId);
		Task<SeatLockReleaseResponse> ReleaseSeatLockAsync(SeatLockReleaseRequest request, string userId);
		Task<SeatLockStatusResponse> GetSeatLockStatusAsync(string lockId, string userId);
	}
}