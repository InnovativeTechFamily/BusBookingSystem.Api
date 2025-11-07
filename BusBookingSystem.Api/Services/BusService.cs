using BusBookingSystem.Api.Data;
using BusBookingSystem.Api.Models.DTOs.Bus;
using BusBookingSystem.Api.Services;

using BusBookingSystem.Api.Models.DTOs.Seat;
using Microsoft.EntityFrameworkCore;

namespace BusBookingSystem.Api.Services
{
	public class BusService : IBusService
	{
		private readonly ApplicationDbContext _context;
		

		public BusService(ApplicationDbContext context)
		{
			_context = context;
			
		}

		public async Task<BusSearchResponse> SearchBusesAsync(BusSearchRequest request)
		{
			try
			{
				

				var buses = await _context.Buses
					.Include(b => b.Operator)
					.Include(b => b.Routes)
					.Include(b => b.Amenities)
					.Include(b => b.BoardingPoints)
					.Include(b => b.DroppingPoints)
					.Where(b => b.Routes.Any(r =>
						r.FromCityId == request.FromCityId &&
						r.ToCityId == request.ToCityId &&
						r.IsActive) && b.IsActive)
					.ToListAsync();

				var busDtos = new List<BusDto>();

				foreach (var bus in buses)
				{
					var route = bus.Routes.First(r => r.FromCityId == request.FromCityId && r.ToCityId == request.ToCityId);
					var availableSeats = await GetAvailableSeatsCount(bus.BusId, request.JourneyDate);

					busDtos.Add(new BusDto
					{
						BusId = bus.BusId,
						BusName = bus.BusName,
						BusNumber = bus.BusNumber,
						BusType = bus.BusType,
						OperatorId = bus.OperatorId,
						OperatorName = bus.Operator.Name,
						Rating = bus.Operator.Rating,
						TotalReviews = bus.Operator.TotalReviews,
						DepartureTime = route.DepartureTime,
						ArrivalTime = route.ArrivalTime,
						Duration = route.Duration,
						DepartureLocation = await GetCityName(request.FromCityId),
						ArrivalLocation = await GetCityName(request.ToCityId),
						TotalSeats = bus.TotalSeats,
						AvailableSeats = availableSeats,
						BaseFare = route.BaseFare,
						IsPremium = bus.IsPremium,
						Amenities = bus.Amenities.Where(a => a.IsActive)
							.Select(a=> a.Name
								//Description = a.Description,
								//IconUrl = a.IconUrl
							).ToList(),
						CancellationPolicy = "Free cancellation before 24 hours",
						BoardingPoints = bus.BoardingPoints.Where(bp => bp.IsActive)
							.Select(bp => new BoardingPointDto
							{
								PointId = bp.PointId,
								Name = bp.Name,
								Address = bp.Address,
								Time = bp.Time.ToString(@"hh\:mm\:ss"),
								Landmark = bp.Landmark
							}).ToList(),
						DroppingPoints = bus.DroppingPoints.Where(dp => dp.IsActive)
							.Select(dp => new DroppingPointDto
							{
								PointId = dp.PointId,
								Name = dp.Name,
								Address = dp.Address,
								Time = dp.Time.ToString(@"hh\:mm\:ss"),
								Landmark = dp.Landmark
							}).ToList()
					});
				}

				var response = new BusSearchResponse
				{
					Success = true,
					Message = "Buses found successfully",
					Data = new BusSearchData
					{
						SearchId = $"SEARCH_{DateTime.UtcNow:yyyyMMdd}_{Guid.NewGuid():N}",
						TotalBuses = busDtos.Count,
						Buses = busDtos
					}
				};

					return response;
			}
			catch (Exception ex)
			{
				return new BusSearchResponse
				{
					Success = false,
					Message = $"Search failed: {ex.Message}"
				};
			}
		}

		public async Task<SeatLayoutResponse> GetSeatLayoutAsync(string busId, DateTime journeyDate)
		{
			try
			{
				var cacheKey = $"seat_layout_{busId}_{journeyDate:yyyyMMdd}";
			

				var bus = await _context.Buses
					.Include(b => b.Seats)
					.FirstOrDefaultAsync(b => b.BusId == busId && b.IsActive);

				if (bus == null)
				{
					return new SeatLayoutResponse
					{
						Success = false,
						Message = "Bus not found"
					};
				}

				// Get locked and booked seats for this journey
				var lockedSeats = await _context.SeatLocks
					.Where(sl => sl.BusId == busId &&
								 sl.JourneyDate == journeyDate.Date &&
								 sl.Status == "Active" &&
								 sl.ExpiresAt > DateTime.UtcNow)
					.Select(sl => sl.SeatId)
					.ToListAsync();

				var bookedSeats = await _context.BookingPassengers
					.Include(bp => bp.Booking)
					.Where(bp => bp.Booking.BusId == busId &&
								bp.Booking.JourneyDate == journeyDate.Date &&
								bp.Booking.Status == "Confirmed")
					.Select(bp => bp.SeatId)
					.ToListAsync();

				var decks = bus.Seats
					.Where(s => s.IsActive)
					.GroupBy(s => s.Deck)
					.Select(g => new DeckDto
					{
						DeckType = g.Key,
						TotalSeats = g.Count(),
						AvailableSeats = g.Count(s => !lockedSeats.Contains(s.SeatId) && !bookedSeats.Contains(s.SeatId)),
						Seats = g.Select(s => new SeatDto
						{
							SeatId = s.SeatId,
							SeatNumber = s.SeatNumber,
							RowNumber = s.RowNumber,
							ColumnNumber = s.ColumnNumber,
							Deck = s.Deck,
							SeatType = s.SeatType,
							Price = s.BasePrice,
							Status = bookedSeats.Contains(s.SeatId) ? "Booked" :
									lockedSeats.Contains(s.SeatId) ? "Locked" : "Available",
							IsLadiesOnly = s.IsLadiesOnly,
							IsWindowSeat = s.IsWindowSeat,
							IsPremium = s.IsPremium,
							Position = new SeatPositionDto
							{
								Row = s.RowNumber,
								Column = s.ColumnNumber,
								Side = s.PositionSide
							},
							BookedBy = bookedSeats.Contains(s.SeatId) ? "Hidden" : null
						}).ToList()
					}).ToList();

				var response = new SeatLayoutResponse
				{
					Success = true,
					Message = "Seat layout retrieved successfully",
					Data = new SeatLayoutData
					{
						BusId = bus.BusId,
						BusName = bus.BusName,
						JourneyDate = journeyDate,
						LayoutType = bus.LayoutType,
						TotalSeats = bus.TotalSeats,
						AvailableSeats = decks.Sum(d => d.AvailableSeats),
						Decks = decks,
						LastUpdated = DateTime.UtcNow
					}
				};

			
				return response;
			}
			catch (Exception ex)
			{
				return new SeatLayoutResponse
				{
					Success = false,
					Message = $"Failed to retrieve seat layout: {ex.Message}"
				};
			}
		}

		private async Task<int> GetAvailableSeatsCount(string busId, DateTime journeyDate)
		{
			var totalSeats = await _context.Seats.CountAsync(s => s.BusId == busId && s.IsActive);

			var lockedSeats = await _context.SeatLocks
				.Where(sl => sl.BusId == busId &&
							sl.JourneyDate == journeyDate.Date &&
							sl.Status == "Active" &&
							sl.ExpiresAt > DateTime.UtcNow)
				.CountAsync();

			var bookedSeats = await _context.BookingPassengers
				.Include(bp => bp.Booking)
				.Where(bp => bp.Booking.BusId == busId &&
							bp.Booking.JourneyDate == journeyDate.Date &&
							bp.Booking.Status == "Confirmed")
				.CountAsync();

			return totalSeats - lockedSeats - bookedSeats;
		}

		private async Task<string> GetCityName(string cityId)
		{
			var city = await _context.Cities.FindAsync(cityId);
			return city?.Name ?? "Unknown City";
		}
	}
}