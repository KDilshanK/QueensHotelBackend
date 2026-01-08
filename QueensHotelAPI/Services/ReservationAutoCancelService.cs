using Microsoft.EntityFrameworkCore;
using QueensHotelAPI.Data;
using QueensHotelAPI.Models;
using System.Globalization;
using Microsoft.Extensions.Logging;

public class ReservationAutoCancelService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReservationAutoCancelService> _logger;
    // Static property to hold the time of day for the next run (default 19:00)
    public static TimeSpan NextRunTimeOfDay { get; set; } = new TimeSpan(19, 00, 0);

    public ReservationAutoCancelService(IServiceScopeFactory scopeFactory, ILogger<ReservationAutoCancelService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ReservationAutoCancelService started at {Time}", DateTime.Now);
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRun = now.Date.Add(NextRunTimeOfDay);
            if (now > nextRun) nextRun = nextRun.AddDays(1);

            var delay = nextRun - now;
            _logger.LogInformation("Next auto-cancel scheduled for {NextRun} (in {Delay} hh:mm:ss)", nextRun, delay);
            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("ReservationAutoCancelService delay cancelled at {Time}", DateTime.Now);
                break;
            }

            _logger.LogInformation("ReservationAutoCancelService executing at {Time}", DateTime.Now);
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<QueensHotelDbContext>();

                    var noShowReservations = await db.Reservation
                        .Where(r =>
                            r.PaymentMethodI_Id == 0 &&
                            r.Status != "Cancelled" &&
                            r.Status != "Completed" &&
                            r.CheckInDate <= DateTime.Today &&
                            !db.Billing.Any(b => b.Reservation_ID == r.Id && b.IsNoShowCharge)
                        )
                        .ToListAsync();

                    _logger.LogInformation("Found {Count} no-show reservations to process.", noShowReservations.Count);

                    foreach (var reservation in noShowReservations)
                    {
                        var billing = new Billing
                        {
                            BillingDate = DateTime.Today,
                            TotalAmount = 0, // TODO: Calculate the no-show charge as per business rules
                            IsNoShowCharge = true,
                            PaymentStatus = "Unpaid",
                            PaymentMethod = null,
                            CreatedBy = "System",
                            CreatedDatetime = DateTime.Now,
                            Reservation_ID = reservation.Id,
                            CompanyMaster_Id = (int)reservation.CompanyMaster_Id
                        };
                        db.Billing.Add(billing);
                        _logger.LogInformation("Added no-show billing for reservation {ReservationId}", reservation.Id);
                    }

                    await db.SaveChangesAsync();
                    _logger.LogInformation("Saved billing changes for no-show reservations.");

                    var cancelIds = noShowReservations.Select(r => r.Id).ToList();
                    if (cancelIds.Any())
                    {
                        var toCancel = db.Reservation.Where(r => cancelIds.Contains(r.Id));
                        foreach (var res in toCancel)
                        {
                            res.Status = "Cancelled";
                            res.CreatedDateTime = DateTime.Now;
                            _logger.LogInformation("Cancelled reservation {ReservationId}", res.Id);
                        }
                        await db.SaveChangesAsync();
                        _logger.LogInformation("Saved cancellation changes for reservations.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during ReservationAutoCancelService execution at {Time}", DateTime.Now);
            }
        }
        _logger.LogInformation("ReservationAutoCancelService stopped at {Time}", DateTime.Now);
    }
}