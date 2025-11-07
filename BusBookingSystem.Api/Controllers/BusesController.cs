using BusBookingSystem.Api.Models.DTOs.Bus;
using BusBookingSystem.Api.Services;
using BusBookingSystem.Api.Models.DTOs.Seat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingSystem.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class BusesController : ControllerBase
	{
		private readonly IBusService _busService;

		public BusesController(IBusService busService)
		{
			_busService = busService;
		}

		[HttpPost("search")]
		public async Task<IActionResult> SearchBuses([FromBody] BusSearchRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new BusSearchResponse
				{
					Success = false,
					Message = "Invalid request data"
				});
			}

			var result = await _busService.SearchBusesAsync(request);

			if (!result.Success)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpGet("{busId}/seat-layout")]
		public async Task<IActionResult> GetSeatLayout(string busId, [FromQuery] DateTime journeyDate)
		{
			if (string.IsNullOrEmpty(busId) || journeyDate == default)
			{
				return BadRequest(new SeatLayoutResponse
				{
					Success = false,
					Message = "Bus ID and journey date are required"
				});
			}

			var result = await _busService.GetSeatLayoutAsync(busId, journeyDate);

			if (!result.Success)
			{
				return NotFound(result);
			}

			return Ok(result);
		}
	}
}