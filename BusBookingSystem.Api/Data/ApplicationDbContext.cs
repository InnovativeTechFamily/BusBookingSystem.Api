using BusBookingSystem.Api.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BusBookingSystem.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<BusOperator> BusOperators { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<BusRoute> BusRoutes { get; set; }
        public DbSet<BoardingPoint> BoardingPoints { get; set; }
        public DbSet<DroppingPoint> DroppingPoints { get; set; }
        public DbSet<SeatLock> SeatLocks { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingPassenger> BookingPassengers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<BusAmenity> BusAmenities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure primary keys
            builder.Entity<City>().HasKey(c => c.CityId);
            builder.Entity<BusOperator>().HasKey(o => o.OperatorId);
            builder.Entity<Bus>().HasKey(b => b.BusId);
            builder.Entity<Seat>().HasKey(s => s.SeatId);
            builder.Entity<BusRoute>().HasKey(r => r.RouteId);
            builder.Entity<BoardingPoint>().HasKey(bp => bp.PointId);
            builder.Entity<DroppingPoint>().HasKey(dp => dp.PointId);
            builder.Entity<SeatLock>().HasKey(sl => sl.LockId);
            builder.Entity<Booking>().HasKey(b => b.BookingId);
            builder.Entity<BookingPassenger>().HasKey(bp => bp.PassengerId);
            builder.Entity<Payment>().HasKey(p => p.PaymentId);
            builder.Entity<BusAmenity>().HasKey(ba => ba.AmenityId);

            // Configure relationships
            builder.Entity<Bus>()
                .HasOne(b => b.Operator)
                .WithMany(o => o.Buses)
                .HasForeignKey(b => b.OperatorId);

            builder.Entity<Seat>()
                .HasOne(s => s.Bus)
                .WithMany(b => b.Seats)
                .HasForeignKey(s => s.BusId);

            builder.Entity<BusRoute>()
                .HasOne(r => r.Bus)
                .WithMany(b => b.Routes)
                .HasForeignKey(r => r.BusId);

            builder.Entity<BusRoute>()
                .HasOne(r => r.FromCity)
                .WithMany(c => c.FromRoutes)
                .HasForeignKey(r => r.FromCityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BusRoute>()
                .HasOne(r => r.ToCity)
                .WithMany(c => c.ToRoutes)
                .HasForeignKey(r => r.ToCityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ensure BoardingPoint and DroppingPoint FKs to Bus do not cascade-delete (prevent multiple cascade paths)
            builder.Entity<BoardingPoint>()
                .HasOne(bp => bp.Bus)
                .WithMany(b => b.BoardingPoints)
                .HasForeignKey(bp => bp.BusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DroppingPoint>()
                .HasOne(dp => dp.Bus)
                .WithMany(b => b.DroppingPoints)
                .HasForeignKey(dp => dp.BusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure SeatLock relationships explicitly to avoid multiple cascade paths
            builder.Entity<SeatLock>()
                .HasOne(sl => sl.User)
                .WithMany()
                .HasForeignKey(sl => sl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SeatLock>()
                .HasOne(sl => sl.Bus)
                .WithMany()
                .HasForeignKey(sl => sl.BusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SeatLock>()
                .HasOne(sl => sl.Seat)
                .WithMany(s => s.SeatLocks)
                .HasForeignKey(sl => sl.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SeatLock>()
                .HasOne(sl => sl.BoardingPoint)
                .WithMany()
                .HasForeignKey(sl => sl.BoardingPointId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SeatLock>()
                .HasOne(sl => sl.DroppingPoint)
                .WithMany()
                .HasForeignKey(sl => sl.DroppingPointId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId);

            // Configure Booking relationships explicitly and set DeleteBehavior.Restrict to avoid multiple cascade paths
            builder.Entity<Booking>()
                .HasOne(b => b.Bus)
                .WithMany()
                .HasForeignKey(b => b.BusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.BoardingPoint)
                .WithMany(bp => bp.Bookings)
                .HasForeignKey(b => b.BoardingPointId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.DroppingPoint)
                .WithMany(dp => dp.Bookings)
                .HasForeignKey(b => b.DroppingPointId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure indexes
            builder.Entity<SeatLock>()
                .HasIndex(sl => new { sl.BusId, sl.JourneyDate, sl.SeatId });

            builder.Entity<Booking>()
                .HasIndex(b => b.PNR)
                .IsUnique();

            builder.Entity<Payment>()
                .HasIndex(p => p.OrderId)
                .IsUnique();

            // Configure decimal precision
            builder.Entity<BusRoute>()
                .Property(r => r.BaseFare)
                .HasPrecision(18, 2);

            builder.Entity<Seat>()
                .Property(s => s.BasePrice)
                .HasPrecision(18, 2);

            builder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasPrecision(18, 2);

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            // BusAmenity relationships
            builder.Entity<BusAmenity>()
                .HasOne(ba => ba.Bus)
                .WithMany(b => b.Amenities)
                .HasForeignKey(ba => ba.BusId);
        }
    }
}
