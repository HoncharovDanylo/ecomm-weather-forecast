using DomainEntities.Entities;

namespace Repositories.Interfaces;

public interface ISelectedCityRepository
{
    IQueryable<SelectedCity> GetAll();

    Task UpdateAsync(SelectedCity city, CancellationToken cancellationToken);
    
    Task AddAsync(SelectedCity city, CancellationToken cancellationToken);
    
    Task DeleteAsync(SelectedCity city, CancellationToken cancellationToken);
}