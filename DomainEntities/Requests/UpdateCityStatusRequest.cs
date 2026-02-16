using DomainEntities.Enums;

namespace DomainEntities.Requests;

public class UpdateCityStatusRequest
{
    public Guid Id { get; set; }
    
    public CityStatus Status { get; set; }
}