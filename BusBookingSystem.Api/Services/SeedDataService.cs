using BusBookingSystem.Api.Data;
using BusBookingSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusBookingSystem.Api.Services
{
    public class SeedDataService
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            // Seed amenities if none exist
            if (!await context.BusAmenities.AnyAsync())
            {
                var amenities = new List<BusAmenity>
                {
                    new BusAmenity { AmenityId = "AMN_001", Name = "WiFi", Description = "Free WiFi", IconUrl = "/icons/wifi.png" },
                    new BusAmenity { AmenityId = "AMN_002", Name = "Charging", Description = "USB Charging Ports", IconUrl = "/icons/charging.png" },
                    new BusAmenity { AmenityId = "AMN_003", Name = "Blanket", Description = "Comfortable Blankets", IconUrl = "/icons/blanket.png" },
                    new BusAmenity { AmenityId = "AMN_004", Name = "Entertainment", Description = "Personal Entertainment", IconUrl = "/icons/entertainment.png" },
                    new BusAmenity { AmenityId = "AMN_005", Name = "Water", Description = "Complimentary Water", IconUrl = "/icons/water.png" },
                    new BusAmenity { AmenityId = "AMN_006", Name = "Snacks", Description = "Light Snacks", IconUrl = "/icons/snacks.png" },
                    new BusAmenity { AmenityId = "AMN_007", Name = "GPS", Description = "Live GPS Tracking", IconUrl = "/icons/gps.png" },
                    new BusAmenity { AmenityId = "AMN_008", Name = "AC", Description = "Air Conditioning", IconUrl = "/icons/ac.png" },
                    new BusAmenity { AmenityId = "AMN_009", Name = "Pillow", Description = "Comfortable Pillows", IconUrl = "/icons/pillow.png" },
                    new BusAmenity { AmenityId = "AMN_010", Name = "Reading Light", Description = "Personal Reading Light", IconUrl = "/icons/reading-light.png" }
                };

                await context.BusAmenities.AddRangeAsync(amenities);
                await context.SaveChangesAsync();
            }
        }
    }
}
