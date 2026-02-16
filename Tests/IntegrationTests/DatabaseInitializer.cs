using DomainEntities.Entities;
using DomainEntities.Enums;
using Infrastructure;

namespace Tests.IntegrationTests;

using System;
using System.Linq;
using DomainEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class DatabaseInitializer
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;
            }

            var customers = new[]
            {
                new User
                {
                    Id =  new Guid("a0e1f2ba-cd18-481c-b165-29ab5c73cc3e"),
                    ChatId = 12345,
                    Username = "John Doe"
                },
                new User { Id = new Guid("ddc8f4b0-9ed8-4089-ba1b-8ed7e618e0dc"), ChatId = 67890, Username = "Jane Smith" }
            };
            context.Users.AddRange(customers);

            var cities = new[]
            {
                new SelectedCity() {Id=new Guid("bf8f1289-2191-410f-bd53-346753b45f31"), CityName = "Kyiv", FormatedCityName = "Kyiv, Ukraine", Latitude = 50.4501, Longitude = 30.5234, UserId = customers[0].Id, Status = CityStatus.Confirmed},
                new SelectedCity() {Id = new Guid("256a2b96-46ef-4573-819b-457a189d96ad"), CityName = "Lviv", FormatedCityName = "Lviv, Ukraine", Latitude = 49.8397, Longitude = 24.0297, UserId = customers[1].Id, Status = CityStatus.PendingForConfirmation},
                new SelectedCity() {Id = new Guid("ddd00185-e1bc-4ddb-bb68-e4f0e3210d88"), CityName = "Lviv", FormatedCityName = "Lviv, Ukraine", Latitude = 49.8397, Longitude = 24.0297, UserId = customers[0].Id, Status = CityStatus.Confirmed}
            };
            context.SelectedCities.AddRange(cities);

            context.SaveChanges();
        }
    }
}