using DomainEntities.Entities;
using Infrastructure;

namespace Repositories.Interfaces;

public interface IUserRepository
{
    IQueryable<User> GetAll();

    Task UpdateAsync(User user);
    
    Task AddAsync(User user, CancellationToken cancellationToken);
    
    Task DeleteAsync(User user);
}