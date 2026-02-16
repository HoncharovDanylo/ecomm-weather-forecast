namespace DomainEntities.Entities;

public class User : BaseEntity
{
    public long ChatId { get; set; }
    
    public string Username { get; set; }

    public List<SelectedCity> SelectedCities { get; set; }
}