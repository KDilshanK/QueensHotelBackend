using System.Collections.Generic;
using System.Threading.Tasks;
using QueensHotelAPI.DTOs;

public interface IFloorService
{
    Task<IEnumerable<FloorDto>> GetAllFloorsAsync();
}