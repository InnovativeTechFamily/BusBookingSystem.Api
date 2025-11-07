using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BusBookingSystem.Api.Interfaces;
using BusBookingSystem.Api.Models.DTOs.Seat;

namespace BusBookingSystem.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class SeatsController : ControllerBase
	{
		private readonly ISeatService _seatService;

		public SeatsController(ISeatService seatService)
		{
			_seatService = seatService;
		}

		[HttpPost("lock")]
		public async Task<IActionResult> LockSeats([FromBody] SeatLockRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new SeatLockResponse
				{
					Success = false,
					Message = "Invalid request data"
				});
			}

			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var result = await _seatService.LockSeatsAsync(request, userId);

			if (!result.Success)
			{
				if (result.Errors?.Any(e => e.Status == "LockedByAnotherUser" || e.Status == "Booked") == true)
				{
					return Conflict(result);
				}
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpPost("lock/extend")]
		public async Task<IActionResult> ExtendSeatLock([FromBody] SeatLockExtendRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new SeatLockResponse
				{
					Success = false,
					Message = "Invalid request data"
				});
			}

			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var result = await _seatService.ExtendSeatLockAsync(request, userId);

			if (!result.Success)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpPost("lock/release")]
		public async Task<IActionResult> ReleaseSeatLock([FromBody] SeatLockReleaseRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new SeatLockReleaseResponse
				{
					Success = false,
					Message = "Invalid request data"
				});
			}

			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var result = await _seatService.ReleaseSeatLockAsync(request, userId);

			if (!result.Success)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpGet("lock/{lockId}/status")]
		public async Task<IActionResult> GetSeatLockStatus(string lockId)
		{
			if (string.IsNullOrEmpty(lockId))
			{
				return BadRequest(new SeatLockStatusResponse
				{
					Success = false,
					Message = "Lock ID is required"
				});
			}

			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var result = await _seatService.GetSeatLockStatusAsync(lockId, userId);

			if (!result.Success)
			{
				return NotFound(result);
			}

			return Ok(result);
		}
	}
}