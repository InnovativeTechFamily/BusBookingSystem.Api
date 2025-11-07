using BusBookingSystem.Api.Data;
using BusBookingSystem.Api.Interfaces;
using BusBookingSystem.Api.Models.DTOs.Bus;
using BusBookingSystem.Api.Models.DTOs.Seat;
using BusBookingSystem.Api.Models.Entities;

using Microsoft.EntityFrameworkCore;

namespace BusBookingSystem.Api.Services
{
	public class SeatService : ISeatService
	{
		private readonly ApplicationDbContext _context;
	

		public SeatService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<SeatLockResponse> LockSeatsAsync(SeatLockRequest request, string userId)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				var errors = new List<SeatLockError>();
				var lockedSeats = new List<LockedSeatDto>();
				var totalBaseFare = 0m;

				// Check each seat for availability
				foreach (var seatId in request.SeatIds)
				{
					var seat = await _context.Seats
						.Include(s => s.Bus)
						.FirstOrDefaultAsync(s => s.SeatId == seatId && s.IsActive);

					if (seat == null)
					{
						errors.Add(new SeatLockError
						{
							SeatId = seatId,
							SeatNumber = "Unknown",
							Status = "NotFound"
						});
						continue;
					}

					// Check if seat is already locked or booked
					var isAvailable = await IsSeatAvailable(seatId, request.BusId, request.JourneyDate);
					if (!isAvailable.IsAvailable)
					{
						errors.Add(new SeatLockError
						{
							SeatId = seatId,
							SeatNumber = seat.SeatNumber,
							Status = isAvailable.Status,
							LockedUntil = isAvailable.LockedUntil
						});
						continue;
					}

					// Seat is available, proceed with locking
					lockedSeats.Add(new LockedSeatDto
					{
						SeatId = seatId,
						SeatNumber = seat.SeatNumber,
						Price = seat.BasePrice
					});

					totalBaseFare += seat.BasePrice;
				}

				// If any seats are not available, return error
				if (errors.Any())
				{
					await transaction.RollbackAsync();
					return new SeatLockResponse
					{
						Success = false,
						Message = "One or more seats are already locked or booked",
						Errors = errors
					};
				}

				// Create seat locks
				var lockId = $"LOCK_{DateTime.UtcNow:yyyyMMdd}_{Guid.NewGuid():N}";
				var sessionId = $"SESSION_{Guid.NewGuid():N}";
				var lockedAt = DateTime.UtcNow;
				var expiresAt = lockedAt.AddMinutes(10);

				foreach (var seatId in request.SeatIds)
				{
					var seatLock = new SeatLock
					{
						LockId = $"{lockId}_{seatId}",
						UserId = userId,
						SessionId = sessionId,
						BusId = request.BusId,
						JourneyDate = request.JourneyDate.Date,
						SeatId = seatId,
						BoardingPointId = request.BoardingPointId,
						DroppingPointId = request.DroppingPointId,
						LockedAt = lockedAt,
						ExpiresAt = expiresAt,
						Status = "Active"
					};

					await _context.SeatLocks.AddAsync(seatLock);

				}

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				// Get boarding and dropping points
				var boardingPoint = await _context.BoardingPoints
					.FirstOrDefaultAsync(bp => bp.PointId == request.BoardingPointId);
				var droppingPoint = await _context.DroppingPoints
					.FirstOrDefaultAsync(dp => dp.PointId == request.DroppingPointId);

				var priceBreakdown = CalculatePriceBreakdown(totalBaseFare);

				return new SeatLockResponse
				{
					Success = true,
					Message = "Seats locked successfully",
					Data = new SeatLockData
					{
						LockId = lockId,
						SessionId = sessionId,
						BusId = request.BusId,
						JourneyDate = request.JourneyDate,
						LockedSeats = lockedSeats,
						BoardingPoint = new BoardingPointDto
						{
							PointId = boardingPoint.PointId,
							Name = boardingPoint.Name,
							Time = boardingPoint.Time.ToString(@"hh\:mm\:ss")
						},
						DroppingPoint = new DroppingPointDto
						{
							PointId = droppingPoint.PointId,
							Name = droppingPoint.Name,
							Time = droppingPoint.Time.ToString(@"hh\:mm\:ss")
						},
						LockExpiresAt = expiresAt,
						LockDurationSeconds = 600,
						PriceBreakdown = priceBreakdown
					}
				};
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				return new SeatLockResponse
				{
					Success = false,
					Message = $"Failed to lock seats: {ex.Message}"
				};
			}
		}

		public async Task<SeatLockResponse> ExtendSeatLockAsync(SeatLockExtendRequest request, string userId)
		{
			try
			{
				var seatLocks = await _context.SeatLocks
					.Where(sl => sl.LockId.StartsWith(request.LockId) &&
								sl.SessionId == request.SessionId &&
								sl.UserId == userId &&
								sl.Status == "Active")
					.ToListAsync();

				if (!seatLocks.Any())
				{
					return new SeatLockResponse
					{
						Success = false,
						Message = "Seat lock not found or expired"
					};
				}

				var newExpiry = DateTime.UtcNow.AddMinutes(15); // Extend by 5 more minutes (10+5)

				foreach (var seatLock in seatLocks)
				{
					seatLock.ExpiresAt = newExpiry;
				}

				await _context.SaveChangesAsync();

				return new SeatLockResponse
				{
					Success = true,
					Message = "Lock extended successfully",
					Data = new SeatLockData
					{
						LockId = request.LockId,
						SessionId = request.SessionId,
						NewExpiryTime = newExpiry,
						RemainingSeconds = (int)(newExpiry - DateTime.UtcNow).TotalSeconds
					}
				};
			}
			catch (Exception ex)
			{
				return new SeatLockResponse
				{
					Success = false,
					Message = $"Failed to extend lock: {ex.Message}"
				};
			}
		}

		public async Task<SeatLockReleaseResponse> ReleaseSeatLockAsync(SeatLockReleaseRequest request, string userId)
		{
			try
			{
				var seatLocks = await _context.SeatLocks
					.Where(sl => sl.LockId.StartsWith(request.LockId) &&
								sl.SessionId == request.SessionId &&
								sl.UserId == userId)
					.ToListAsync();

				if (!seatLocks.Any())
				{
					return new SeatLockReleaseResponse
					{
						Success = false,
						Message = "Seat lock not found"
					};
				}

				var releasedSeats = seatLocks.Select(sl => sl.SeatId).ToList();

				// Remove from database
				_context.SeatLocks.RemoveRange(seatLocks);

				await _context.SaveChangesAsync();

				return new SeatLockReleaseResponse
				{
					Success = true,
					Message = "Seats released successfully",
					Data = new SeatLockReleaseData
					{
						LockId = request.LockId,
						ReleasedSeats = releasedSeats,
						ReleasedAt = DateTime.UtcNow
					}
				};
			}
			catch (Exception ex)
			{
				return new SeatLockReleaseResponse
				{
					Success = false,
					Message = $"Failed to release seats: {ex.Message}"
				};
			}
		}

		public async Task<SeatLockStatusResponse> GetSeatLockStatusAsync(string lockId, string userId)
		{
			try
			{
				var seatLocks = await _context.SeatLocks
					.Where(sl => sl.LockId.StartsWith(lockId) && sl.UserId == userId)
					.ToListAsync();

				if (!seatLocks.Any())
				{
					return new SeatLockStatusResponse
					{
						Success = false,
						Message = "Seat lock not found"
					};
				}

				var firstLock = seatLocks.First();
				var remainingSeconds = (int)(firstLock.ExpiresAt - DateTime.UtcNow).TotalSeconds;
				var canExtend = remainingSeconds < 300; // Can extend if less than 5 minutes remaining

				return new SeatLockStatusResponse
				{
					Success = true,
					Message = "Lock status retrieved",
					Data = new SeatLockStatusData
					{
						LockId = lockId,
						Status = firstLock.Status,
						LockedSeats = seatLocks.Select(sl => sl.SeatId).ToList(),
						ExpiresAt = firstLock.ExpiresAt,
						RemainingSeconds = Math.Max(0, remainingSeconds),
						CanExtend = canExtend
					}
				};
			}
			catch (Exception ex)
			{
				return new SeatLockStatusResponse
				{
					Success = false,
					Message = $"Failed to get lock status: {ex.Message}"
				};
			}
		}

		private async Task<(bool IsAvailable, string Status, DateTime? LockedUntil)> IsSeatAvailable(string seatId, string busId, DateTime journeyDate)
		{
			
			// Check database for more accurate status
			var existingLock = await _context.SeatLocks
				.FirstOrDefaultAsync(sl => sl.SeatId == seatId &&
										 sl.BusId == busId &&
										 sl.JourneyDate == journeyDate.Date &&
										 sl.Status == "Active" &&
										 sl.ExpiresAt > DateTime.UtcNow);

			if (existingLock != null)
			{
				return (false, "LockedByAnotherUser", existingLock.ExpiresAt);
			}

			var isBooked = await _context.BookingPassengers
				.Include(bp => bp.Booking)
				.AnyAsync(bp => bp.SeatId == seatId &&
							   bp.Booking.BusId == busId &&
							   bp.Booking.JourneyDate == journeyDate.Date &&
							   bp.Booking.Status == "Confirmed");

			if (isBooked)
			{
				return (false, "Booked", null);
			}

			return (true, "Available", null);
		}

		private PriceBreakdownDto CalculatePriceBreakdown(decimal baseFare)
		{
			var taxes = baseFare * 0.05m; // 5% tax
			var serviceFee = 50m; // Fixed service fee
			var discount = 0m;

			return new PriceBreakdownDto
			{
				BaseFare = baseFare,
				Taxes = taxes,
				ServiceFee = serviceFee,
				Discount = discount,
				TotalAmount = baseFare + taxes + serviceFee - discount
			};
		}
	}
}