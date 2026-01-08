using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QueensHotelAPI.Data;
using QueensHotelAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using QueensHotelAPI.Repositories;

public class TravelAgencyRepository : ITravelAgencyRepository
{
    private readonly QueensHotelDbContext _context;

    public TravelAgencyRepository(QueensHotelDbContext context)
    {
        _context = context;
    }

    public async Task<int> InsertTravelAgencyAsync(TravelAgencyCreateDto dto)
    {
        var companyMasterIdParam = new SqlParameter("@CompanyMaster_Id", dto.CompanyMaster_Id);
        var agencyNameParam = new SqlParameter("@AgencyName", dto.AgencyName ?? (object)DBNull.Value);
        var contactPersonParam = new SqlParameter("@ContactPerson", dto.ContactPerson ?? (object)DBNull.Value);
        var phoneParam = new SqlParameter("@Phone", dto.Phone ?? (object)DBNull.Value);
        var emailParam = new SqlParameter("@Email", dto.Email ?? (object)DBNull.Value);
        var addressParam = new SqlParameter("@Address", dto.Address ?? (object)DBNull.Value);
        var discountRateParam = new SqlParameter("@DiscountRate", dto.DiscountRate);
        var webpageUrlParam = new SqlParameter("@WebpageUrl", dto.WebpageUrl ?? (object)DBNull.Value);
        var passwordParam = new SqlParameter("@Password", dto.Password ?? (object)DBNull.Value);

        await _context.Database.ExecuteSqlRawAsync(
            "EXEC [dbo].[InsertTravalAgency] @AgencyName, @ContactPerson, @Phone, @Email, @Address, @DiscountRate, @WebpageUrl, @CompanyMaster_Id, @Password",
            agencyNameParam, contactPersonParam, phoneParam, emailParam, addressParam, discountRateParam, webpageUrlParam, companyMasterIdParam, passwordParam
        );
        return 1;
    }

    public async Task<IEnumerable<TravelAgencyDto>> GetTravelAgencyDataAsync(
    string agencyName = null,
    string contactPerson = null,
    string phone = null,
    string email = null,
    string webpageUrl = null)
    {
        var agencyNameParam = new SqlParameter("@AgencyName", agencyName ?? (object)DBNull.Value);
        var contactPersonParam = new SqlParameter("@ContactPerson", contactPerson ?? (object)DBNull.Value);
        var phoneParam = new SqlParameter("@Phone", phone ?? (object)DBNull.Value);
        var emailParam = new SqlParameter("@Email", email ?? (object)DBNull.Value);
        var webpageUrlParam = new SqlParameter("@WebpageUrl", webpageUrl ?? (object)DBNull.Value);

        var agencies = await _context.TravelAgency
            .FromSqlRaw(
                "EXEC [dbo].[GetTravalAgencyData] @AgencyName, @ContactPerson, @Phone, @Email, @WebpageUrl",
                agencyNameParam, contactPersonParam, phoneParam, emailParam, webpageUrlParam
            )
            .ToListAsync();

        var result = agencies.Select(a => new TravelAgencyDto
        {
            Id = a.Id,
            AgencyName = a.AgencyName,
            ContactPerson = a.ContactPerson,
            Phone = a.Phone,
            Email = a.Email,
            Address = a.Address,
            DiscountRate = (double)a.DiscountRate,
            WebpageUrl = a.WebpageUrl,
            CompanyMaster_Id = (int)a.CompanyMaster_Id
        }).ToList();

        return result;
    }
}