using System.Collections.Generic;
using System.Threading.Tasks;
using QueensHotelAPI.DTOs;

public interface IFloorRepository
{
    Task<IEnumerable<FloorDto>> GetAllFloorsAsync();
}