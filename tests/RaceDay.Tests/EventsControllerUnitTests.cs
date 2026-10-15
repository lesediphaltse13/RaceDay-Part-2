using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Controllers;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using Xunit;

namespace RaceDay.Tests;

/// <summary>School-guide-style Arrange–Act–Assert tests using an isolated EF Core InMemory database.</summary>
public class EventsControllerUnitTests
{
    private static RaceDayDbContext NewContext()
    {
        var options = new DbContextOptionsBuilder<RaceDayDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new RaceDayDbContext(options);
    }

    private static RaceEvent SampleEvent() => new()
    {
        EventName = "Pretoria 10K", OrganiserId = 1,
        Description = "Community run", EventType = "Running",
        EventDate = new DateTime(2030, 10, 20, 8, 0, 0, DateTimeKind.Utc),
        RegistrationClosingDate = new DateTime(2030, 10, 19, 8, 0, 0, DateTimeKind.Utc),
        Venue = "City Park", City = "Pretoria", Province = "Gauteng"
    };

    [Fact]
    public async Task GetAll_ReturnsOkAndEvents()
    {
        // Arrange
        await using var db = NewContext();
        db.Events.Add(SampleEvent());
        await db.SaveChangesAsync();
        var controller = new EventsController(db);

        // Act
        var result = await controller.All(null, null);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Single(await db.Events.ToListAsync());
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        await using var db = NewContext();
        var controller = new EventsController(db);

        // Act
        var result = await controller.One(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenEventExists()
    {
        // Arrange
        await using var db = NewContext();
        var item = SampleEvent();
        db.Events.Add(item);
        await db.SaveChangesAsync();
        var controller = new EventsController(db);

        // Act
        var result = await controller.One(item.EventId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        await using var db = NewContext();
        var controller = new EventsController(db);

        // Act
        var result = await controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        await using var db = NewContext();
        var controller = new EventsController(db);
        var request = new EventRequest("Pretoria 10K", "Community run", "Running",
            new DateTime(2030, 10, 20), new DateTime(2030, 10, 19),
            "City Park", "Pretoria", "Gauteng", null);

        // Act
        var result = await controller.Update(999, request);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
