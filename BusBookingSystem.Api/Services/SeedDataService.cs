using BusBookingSystem.Api.Data;
using BusBookingSystem.Api.Models.Entities;
using BusBookingSystem.API.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusBookingSystem.API.Services
{
	public class SeedDataService
	{
		public static async Task Initialize(IServiceProvider serviceProvider)
		{
			using var scope = serviceProvider.CreateScope();
			var services = scope.ServiceProvider;

			var context = services.GetRequiredService<ApplicationDbContext>();
			var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
			var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

			// Ensure database is created
			await context.Database.MigrateAsync();

			// Seed Roles
			await SeedRolesAsync(roleManager);

			// Seed Super Admin User
			await SeedSuperAdminAsync(userManager);

			// Seed Sample Bus Operator
			await SeedBusOperatorAsync(userManager);

			// Seed Cities
			await SeedCitiesAsync(context);

			// Seed Bus Operators
			await SeedBusOperatorsAsync(context);

			// Seed Buses
			await SeedBusesAsync(context);

			// Seed Amenities
			await SeedAmenitiesAsync(context);

			// Seed Routes and Points
			await SeedRoutesAndPointsAsync(context);

			// Seed Seats
			await SeedSeatsAsync(context);
		}

		private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
		{
			foreach (var roleName in RoleConstants.AllRoles)
			{
				var roleExists = await roleManager.RoleExistsAsync(roleName);
				if (!roleExists)
				{
					await roleManager.CreateAsync(new IdentityRole(roleName));
				}
			}
		}

		private static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager)
		{
			var superAdminEmail = "superadmin@swiftbus.com";
			var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);

			if (superAdminUser == null)
			{
				var user = new ApplicationUser
				{
					FullName = "Super Administrator",
					UserName = superAdminEmail,
					Email = superAdminEmail,
					PhoneNumber = "+919999999999",
					EmailConfirmed = true,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow
				};

				var result = await userManager.CreateAsync(user, "SuperAdmin@123");
				if (result.Succeeded)
				{
					await userManager.AddToRolesAsync(user, new[] { RoleConstants.SuperAdmin, RoleConstants.Admin });
				}
			}
		}

		private static async Task SeedBusOperatorAsync(UserManager<ApplicationUser> userManager)
		{
			var operatorEmail = "operator@swifttravels.com";
			var operatorUser = await userManager.FindByEmailAsync(operatorEmail);

			if (operatorUser == null)
			{
				var user = new ApplicationUser
				{
					FullName = "Swift Travels Operator",
					UserName = operatorEmail,
					Email = operatorEmail,
					PhoneNumber = "+919888888888",
					EmailConfirmed = true,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow
				};

				var result = await userManager.CreateAsync(user, "Operator@123");
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(user, RoleConstants.BusOperator);
				}
			}
		}

		private static async Task SeedCitiesAsync(ApplicationDbContext context)
		{
			if (!await context.Cities.AnyAsync())
			{
				var cities = new List<City>
				{
					new City { CityId = "CITY_MUM_001", Name = "Mumbai", State = "Maharashtra", Country = "India" },
					new City { CityId = "CITY_PUN_002", Name = "Pune", State = "Maharashtra", Country = "India" },
					new City { CityId = "CITY_DEL_003", Name = "Delhi", State = "Delhi", Country = "India" },
					new City { CityId = "CITY_BLR_004", Name = "Bengaluru", State = "Karnataka", Country = "India" },
					new City { CityId = "CITY_CHN_005", Name = "Chennai", State = "Tamil Nadu", Country = "India" },
					new City { CityId = "CITY_HYD_006", Name = "Hyderabad", State = "Telangana", Country = "India" },
					new City { CityId = "CITY_AHD_007", Name = "Ahmedabad", State = "Gujarat", Country = "India" },
					new City { CityId = "CITY_KOL_008", Name = "Kolkata", State = "West Bengal", Country = "India" }
				};

				await context.Cities.AddRangeAsync(cities);
				await context.SaveChangesAsync();
			}
		}

		private static async Task SeedBusOperatorsAsync(ApplicationDbContext context)
		{
			if (!await context.BusOperators.AnyAsync())
			{
				var operators = new List<BusOperator>
				{
					new BusOperator
					{
						OperatorId = "OP_001",
						Name = "SwiftBus Travels",
						Email = "info@swifttravels.com",
						Phone = "+919876543210",
						Rating = 4.8m,
						TotalReviews = 245
					},
					new BusOperator
					{
						OperatorId = "OP_002",
						Name = "Royal Luxury Lines",
						Email = "contact@royalluxury.com",
						Phone = "+919876543211",
						Rating = 4.6m,
						TotalReviews = 189
					},
					new BusOperator
					{
						OperatorId = "OP_003",
						Name = "Comfort Express",
						Email = "support@comfortexpress.com",
						Phone = "+919876543212",
						Rating = 4.4m,
						TotalReviews = 156
					},
					new BusOperator
					{
						OperatorId = "OP_004",
						Name = "GreenLine Travels",
						Email = "info@greenline.com",
						Phone = "+919876543213",
						Rating = 4.7m,
						TotalReviews = 203
					}
				};

				await context.BusOperators.AddRangeAsync(operators);
				await context.SaveChangesAsync();
			}
		}

		private static async Task SeedBusesAsync(ApplicationDbContext context)
		{
			if (!await context.Buses.AnyAsync())
			{
				var buses = new List<Bus>
				{
					new Bus
					{
						BusId = "BUS_RL_001",
						BusName = "Royal Luxury Express",
						BusNumber = "MH-01-AB-1234",
						BusType = "AC Sleeper",
						OperatorId = "OP_001",
						TotalSeats = 36,
						LayoutType = "2+1",
						IsPremium = true
					},
					new Bus
					{
						BusId = "BUS_SE_002",
						BusName = "Swift Executive",
						BusNumber = "MH-01-CD-5678",
						BusType = "AC Semi-Sleeper",
						OperatorId = "OP_001",
						TotalSeats = 40,
						LayoutType = "2+2",
						IsPremium = false
					},
					new Bus
					{
						BusId = "BUS_CE_003",
						BusName = "Comfort Express",
						BusNumber = "MH-12-EF-9012",
						BusType = "Non-AC Sleeper",
						OperatorId = "OP_003",
						TotalSeats = 42,
						LayoutType = "2+1",
						IsPremium = false
					},
					new Bus
					{
						BusId = "BUS_GL_004",
						BusName = "GreenLine Premium",
						BusNumber = "MH-12-GH-3456",
						BusType = "AC Sleeper",
						OperatorId = "OP_004",
						TotalSeats = 32,
						LayoutType = "2+1",
						IsPremium = true
					}
				};

				await context.Buses.AddRangeAsync(buses);
				await context.SaveChangesAsync();
			}
		}

		private static async Task SeedAmenitiesAsync(ApplicationDbContext context)
		{
			if (!await context.BusAmenities.AnyAsync())
			{
				var amenities = new List<BusAmenity>
				{
                    // Amenities for Royal Luxury Express
                    new BusAmenity { AmenityId = "AMN_RL_001", BusId = "BUS_RL_001", Name = "WiFi", Description = "Free High-Speed WiFi", IconUrl = "/icons/wifi.png" },
					new BusAmenity { AmenityId = "AMN_RL_002", BusId = "BUS_RL_001", Name = "Charging", Description = "USB Charging Ports", IconUrl = "/icons/charging.png" },
					new BusAmenity { AmenityId = "AMN_RL_003", BusId = "BUS_RL_001", Name = "Blanket", Description = "Comfortable Blankets", IconUrl = "/icons/blanket.png" },
					new BusAmenity { AmenityId = "AMN_RL_004", BusId = "BUS_RL_001", Name = "Entertainment", Description = "Personal Entertainment", IconUrl = "/icons/entertainment.png" },
					new BusAmenity { AmenityId = "AMN_RL_005", BusId = "BUS_RL_001", Name = "Water", Description = "Complimentary Water", IconUrl = "/icons/water.png" },
					new BusAmenity { AmenityId = "AMN_RL_006", BusId = "BUS_RL_001", Name = "Snacks", Description = "Light Snacks", IconUrl = "/icons/snacks.png" },
					new BusAmenity { AmenityId = "AMN_RL_007", BusId = "BUS_RL_001", Name = "GPS", Description = "Live GPS Tracking", IconUrl = "/icons/gps.png" },

                    // Amenities for Swift Executive
                    new BusAmenity { AmenityId = "AMN_SE_001", BusId = "BUS_SE_002", Name = "WiFi", Description = "Free WiFi", IconUrl = "/icons/wifi.png" },
					new BusAmenity { AmenityId = "AMN_SE_002", BusId = "BUS_SE_002", Name = "Charging", Description = "USB Charging Ports", IconUrl = "/icons/charging.png" },
					new BusAmenity { AmenityId = "AMN_SE_003", BusId = "BUS_SE_002", Name = "Water", Description = "Complimentary Water", IconUrl = "/icons/water.png" },

                    // Amenities for Comfort Express
                    new BusAmenity { AmenityId = "AMN_CE_001", BusId = "BUS_CE_003", Name = "Charging", Description = "USB Charging Ports", IconUrl = "/icons/charging.png" },
					new BusAmenity { AmenityId = "AMN_CE_002", BusId = "BUS_CE_003", Name = "Water", Description = "Complimentary Water", IconUrl = "/icons/water.png" },

                    // Amenities for GreenLine Premium
                    new BusAmenity { AmenityId = "AMN_GL_001", BusId = "BUS_GL_004", Name = "WiFi", Description = "Free WiFi", IconUrl = "/icons/wifi.png" },
					new BusAmenity { AmenityId = "AMN_GL_002", BusId = "BUS_GL_004", Name = "Charging", Description = "USB Charging Ports", IconUrl = "/icons/charging.png" },
					new BusAmenity { AmenityId = "AMN_GL_003", BusId = "BUS_GL_004", Name = "Blanket", Description = "Comfortable Blankets", IconUrl = "/icons/blanket.png" },
					new BusAmenity { AmenityId = "AMN_GL_004", BusId = "BUS_GL_004", Name = "Water", Description = "Complimentary Water", IconUrl = "/icons/water.png" },
					new BusAmenity { AmenityId = "AMN_GL_005", BusId = "BUS_GL_004", Name = "GPS", Description = "Live GPS Tracking", IconUrl = "/icons/gps.png" }
				};

				await context.BusAmenities.AddRangeAsync(amenities);
				await context.SaveChangesAsync();
			}
		}

		private static async Task SeedRoutesAndPointsAsync(ApplicationDbContext context)
		{
			if (!await context.BusRoutes.AnyAsync())
			{
				var routes = new List<BusRoute>
				{
                    // Mumbai to Pune routes
                    new BusRoute
					{
						RouteId = "ROUTE_MUM_PUN_001",
						BusId = "BUS_RL_001",
						FromCityId = "CITY_MUM_001",
						ToCityId = "CITY_PUN_002",
						DepartureTime = new TimeSpan(6, 30, 0),
						ArrivalTime = new TimeSpan(10, 0, 0),
						Duration = new TimeSpan(3, 30, 0),
						BaseFare = 1899.00m
					},
					new BusRoute
					{
						RouteId = "ROUTE_MUM_PUN_002",
						BusId = "BUS_SE_002",
						FromCityId = "CITY_MUM_001",
						ToCityId = "CITY_PUN_002",
						DepartureTime = new TimeSpan(8, 0, 0),
						ArrivalTime = new TimeSpan(12, 0, 0),
						Duration = new TimeSpan(4, 0, 0),
						BaseFare = 1299.00m
					},

                    // Pune to Mumbai routes
                    new BusRoute
					{
						RouteId = "ROUTE_PUN_MUM_001",
						BusId = "BUS_RL_001",
						FromCityId = "CITY_PUN_002",
						ToCityId = "CITY_MUM_001",
						DepartureTime = new TimeSpan(18, 0, 0),
						ArrivalTime = new TimeSpan(21, 30, 0),
						Duration = new TimeSpan(3, 30, 0),
						BaseFare = 1899.00m
					},

                    // Mumbai to Delhi routes
                    new BusRoute
					{
						RouteId = "ROUTE_MUM_DEL_001",
						BusId = "BUS_GL_004",
						FromCityId = "CITY_MUM_001",
						ToCityId = "CITY_DEL_003",
						DepartureTime = new TimeSpan(20, 0, 0),
						ArrivalTime = new TimeSpan(8, 0, 0),
						Duration = new TimeSpan(12, 0, 0),
						BaseFare = 2899.00m
					}
				};

				await context.BusRoutes.AddRangeAsync(routes);
				await context.SaveChangesAsync();
			}

			if (!await context.BoardingPoints.AnyAsync())
			{
				var boardingPoints = new List<BoardingPoint>
				{
                    // Mumbai boarding points
                    new BoardingPoint { PointId = "BP_MUM_001", BusId = "BUS_RL_001", Name = "Mumbai Central Station", Address = "Platform 2, Mumbai Central Railway Station", Landmark = "Near Western Express Highway", Time = new TimeSpan(6, 30, 0) },
					new BoardingPoint { PointId = "BP_MUM_002", BusId = "BUS_RL_001", Name = "Dadar West", Address = "Near Shivaji Park, Dadar West", Landmark = "Opposite Dadar Market", Time = new TimeSpan(6, 50, 0) },
					new BoardingPoint { PointId = "BP_MUM_003", BusId = "BUS_SE_002", Name = "Mumbai Central Station", Address = "Platform 3, Mumbai Central Railway Station", Landmark = "Near Western Express Highway", Time = new TimeSpan(8, 0, 0) },
                    
                    // Pune boarding points
                    new BoardingPoint { PointId = "BP_PUN_001", BusId = "BUS_RL_001", Name = "Pune Railway Station", Address = "Main Entrance, Pune Junction", Landmark = "Platform 1 Exit", Time = new TimeSpan(18, 0, 0) },
					new BoardingPoint { PointId = "BP_PUN_002", BusId = "BUS_RL_001", Name = "Hinjewadi IT Park", Address = "Phase 1, Rajiv Gandhi Infotech Park", Landmark = "Near Gate 2", Time = new TimeSpan(18, 30, 0) }
				};

				await context.BoardingPoints.AddRangeAsync(boardingPoints);
				await context.SaveChangesAsync();
			}

			if (!await context.DroppingPoints.AnyAsync())
			{
				var droppingPoints = new List<DroppingPoint>
				{
                    // Pune dropping points
                    new DroppingPoint { PointId = "DP_PUN_001", BusId = "BUS_RL_001", Name = "Pune Railway Station", Address = "Main Entrance, Pune Junction", Landmark = "Platform 1 Exit", Time = new TimeSpan(10, 0, 0) },
					new DroppingPoint { PointId = "DP_PUN_002", BusId = "BUS_RL_001", Name = "Hinjewadi IT Park", Address = "Phase 1, Rajiv Gandhi Infotech Park", Landmark = "Near Gate 2", Time = new TimeSpan(10, 30, 0) },
                    
                    // Mumbai dropping points
                    new DroppingPoint { PointId = "DP_MUM_001", BusId = "BUS_RL_001", Name = "Mumbai Central Station", Address = "Platform 2, Mumbai Central Railway Station", Landmark = "Near Western Express Highway", Time = new TimeSpan(21, 30, 0) },
					new DroppingPoint { PointId = "DP_MUM_002", BusId = "BUS_RL_001", Name = "Dadar West", Address = "Near Shivaji Park, Dadar West", Landmark = "Opposite Dadar Market", Time = new TimeSpan(21, 50, 0) }
				};

				await context.DroppingPoints.AddRangeAsync(droppingPoints);
				await context.SaveChangesAsync();
			}
		}

		private static async Task SeedSeatsAsync(ApplicationDbContext context)
		{
			if (!await context.Seats.AnyAsync())
			{
				var seats = new List<Seat>();

				// Seed seats for Royal Luxury Express (36 seats - 2+1 layout)
				var busId = "BUS_RL_001";
				var seatNumber = 1;

				// Lower Deck - 20 seats
				for (int row = 1; row <= 10; row++)
				{
					for (int col = 1; col <= 2; col++)
					{
						var isWindowSeat = col == 1;
						var positionSide = col == 1 ? "Left" : "Right";

						seats.Add(new Seat
						{
							SeatId = $"SEAT_{busId}_L{seatNumber}",
							BusId = busId,
							SeatNumber = $"L{seatNumber}",
							RowNumber = row,
							ColumnNumber = col,
							Deck = "Lower",
							SeatType = "Sleeper",
							BasePrice = 1899.00m,
							IsLadiesOnly = false,
							IsWindowSeat = isWindowSeat,
							IsPremium = true,
							PositionSide = positionSide
						});
						seatNumber++;
					}
				}

				// Upper Deck - 16 seats
				seatNumber = 1;
				for (int row = 1; row <= 8; row++)
				{
					for (int col = 1; col <= 2; col++)
					{
						var isWindowSeat = col == 1;
						var positionSide = col == 1 ? "Left" : "Right";

						seats.Add(new Seat
						{
							SeatId = $"SEAT_{busId}_U{seatNumber}",
							BusId = busId,
							SeatNumber = $"U{seatNumber}",
							RowNumber = row,
							ColumnNumber = col,
							Deck = "Upper",
							SeatType = "Premium Sleeper",
							BasePrice = 2199.00m,
							IsLadiesOnly = false,
							IsWindowSeat = isWindowSeat,
							IsPremium = true,
							PositionSide = positionSide
						});
						seatNumber++;
					}
				}

				// Mark some seats as ladies only
				seats[2].IsLadiesOnly = true;  // L3
				seats[5].IsLadiesOnly = true;  // L6
				seats[8].IsLadiesOnly = true;  // L9

				await context.Seats.AddRangeAsync(seats);
				await context.SaveChangesAsync();
			}
		}
	}
}