using Microsoft.EntityFrameworkCore;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Models;

namespace QueensHotelAPI.Data
{
    public class QueensHotelDbContext : DbContext
    {
        public QueensHotelDbContext(DbContextOptions<QueensHotelDbContext> options) : base(options)
        {
            Console.WriteLine($"Queens Hotel API: DbContext constructor called at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        }

        // Basic DbSet properties
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Reservation> Reservation { get; set; }
        public DbSet<MealPlan> MealPlans { get; set; }
        public DbSet<Suite> Suites { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<EnumRoomStatus> EnumRoomStatuses { get; set; }
        public DbSet<TravelAgency> TravelAgencies { get; set; }
        public DbSet<PaymentCard> PaymentCards { get; set; }
        public DbSet<CardType> CardTypes { get; set; }

        // Keyless entities for stored procedure results
        public DbSet<ReservationDetailsResult> ReservationDetailsResults { get; set; }
        public DbSet<CustomerDetailsResult> CustomerDetailsResults { get; set; }
        public DbSet<MealPlan> MealPlanDetails{ get; set; }
        public DbSet<RoomDetailsResult> RoomDetailsResults { get; set; }
        public DbSet<SuiteDetailsResult> SuiteDetailsResults { get; set; }
        public DbSet<Billing> Billing { get; set; }
        public DbSet<TravelAgency> TravelAgency { get; set; }
        public DbSet<CheckOutResultDto> CheckOutResultDto { get; set; }
        public DbSet<CheckOutDetailsDto> CheckOutDetailsDto { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            try
            {
                Console.WriteLine($"Queens Hotel API: OnModelCreating started at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");

                // Basic table configuration
                modelBuilder.Entity<Customer>().ToTable("Customer");
                modelBuilder.Entity<Reservation>().ToTable("Reservation");
                modelBuilder.Entity<MealPlan>().ToTable("MealPlan");
                modelBuilder.Entity<Suite>().ToTable("Suite");
                modelBuilder.Entity<Room>().ToTable("Room");
                modelBuilder.Entity<Floor>().ToTable("Floor");
                modelBuilder.Entity<RoomType>().ToTable("RoomType");
                modelBuilder.Entity<EnumRoomStatus>().ToTable("ENUM_RoomStatus");
                modelBuilder.Entity<TravelAgency>().ToTable("TravalAgency");
                modelBuilder.Entity<PaymentCard>().ToTable("PaymentCard");
                modelBuilder.Entity<CardType>().ToTable("CardType");

                // Configure keyless entities for stored procedure results
                modelBuilder.Entity<ReservationDetailsResult>().HasNoKey();
                modelBuilder.Entity<CustomerDetailsResult>().HasNoKey();
                modelBuilder.Entity<MealPlan>().HasNoKey();
                modelBuilder.Entity<RoomDetailsResult>().HasNoKey();
                modelBuilder.Entity<SuiteDetailsResult>().HasNoKey();
                modelBuilder.Entity<CheckOutResultDto>().HasNoKey();
                modelBuilder.Entity<CheckOutDetailsDto>().HasNoKey();

                // =====================================================
                // Configure decimal precision for all decimal properties
                // Using precision(18, 2) for monetary values
                // Using precision(5, 2) for percentage/rate values
                // =====================================================

                // CheckOutDetailsDto decimal properties
                modelBuilder.Entity<CheckOutDetailsDto>()
                    .Property(e => e.SuiteMonthlyRate)
                    .HasPrecision(18, 2);

                modelBuilder.Entity<CheckOutDetailsDto>()
                    .Property(e => e.SuiteWeeklyRate)
                    .HasPrecision(18, 2);

                // CheckOutResultDto decimal properties
                modelBuilder.Entity<CheckOutResultDto>()
                    .Property(e => e.AccommodationCharge)
                    .HasPrecision(18, 2);

                modelBuilder.Entity<CheckOutResultDto>()
                    .Property(e => e.TotalBilled)
                    .HasPrecision(18, 2);

                // MealPlan decimal properties
                modelBuilder.Entity<MealPlan>()
                    .Property(e => e.CostPerNight)
                    .HasPrecision(18, 2);

                // ReservationDetailsResult decimal properties
                modelBuilder.Entity<ReservationDetailsResult>()
                    .Property(e => e.CostPerNight)
                    .HasPrecision(18, 2);

                modelBuilder.Entity<ReservationDetailsResult>()
                    .Property(e => e.DiscountRate)
                    .HasPrecision(5, 2);

                modelBuilder.Entity<ReservationDetailsResult>()
                    .Property(e => e.RoomRatePerNight)
                    .HasPrecision(18, 2);

                modelBuilder.Entity<ReservationDetailsResult>()
                    .Property(e => e.SuiteMonthlyRate)
                    .HasPrecision(18, 2);

                modelBuilder.Entity<ReservationDetailsResult>()
                    .Property(e => e.SuiteWeeklyRate)
                    .HasPrecision(18, 2);

                // RoomDetailsResult decimal properties
                modelBuilder.Entity<RoomDetailsResult>()
                    .Property(e => e.RatePerNight)
                    .HasPrecision(18, 2);

                // RoomType decimal properties
                modelBuilder.Entity<RoomType>()
                    .Property(e => e.RatePerNight)
                    .HasPrecision(18, 2);

                // Suite decimal properties
                modelBuilder.Entity<Suite>()
                    .Property(e => e.MonthlyRate)
                    .HasPrecision(18, 2);

                modelBuilder.Entity<Suite>()
                    .Property(e => e.WeeklyRate)
                    .HasPrecision(18, 2);

                // SuiteDetailsResult decimal properties
                modelBuilder.Entity<SuiteDetailsResult>()
                    .Property(e => e.MonthlyRate)
                    .HasPrecision(18, 2);

                modelBuilder.Entity<SuiteDetailsResult>()
                    .Property(e => e.WeeklyRate)
                    .HasPrecision(18, 2);

                Console.WriteLine($"Queens Hotel API: OnModelCreating completed at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");

                base.OnModelCreating(modelBuilder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Queens Hotel API: OnModelCreating failed at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {ex.Message}");
                throw;
            }
        }
    }
}