using System.Collections.Generic;
using System.Threading.Tasks;
using QueensHotelAPI.DTOs;
using QueensHotelAPI.Repositories;

public class FloorService : IFloorService
{
    private readonly IFloorRepository _repository;

    public FloorService(IFloorRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FloorDto>> GetAllFloorsAsync()
    {
        return await _repository.GetAllFloorsAsync();
    }
}