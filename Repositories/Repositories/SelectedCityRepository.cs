using DomainEntities.Entities;
using Infrastructure;
using Repositories.Interfaces;

namespace Repositories.Repositories;

public class SelectedCityRepository : ISelectedCityRepository
{
    private readonly AppDbContext _context;

    public SelectedCityRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<SelectedCity> GetAll()
    {
        return _context.SelectedCities;
    }

    public async Task UpdateAsync(SelectedCity city, CancellationToken cancellationToken)
    {
        _context.SelectedCities.Update(city);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(SelectedCity city, CancellationToken cancellationToken)
    {
        _context.SelectedCities.Add(city);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(SelectedCity city, CancellationToken cancellationToken)
    {
        city.IsDeleted = true;
        city.DeletedAt = DateTime.UtcNow;
        _context.Update(city);
        await _context.SaveChangesAsync();
    }
}