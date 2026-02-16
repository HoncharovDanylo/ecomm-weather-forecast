using DomainEntities.Enums;

namespace DomainEntities.Entities;

public class SelectedCity : BaseEntity
{
    public string CityName { get; set; }
    
    public string FormatedCityName { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    public CityStatus Status { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; }
}