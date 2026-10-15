
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using Xunit;

namespace RaceDay.Tests;

public class ApiTests : IClassFixture<RaceDayFactory>
{
    private readonly RaceDayFactory _factory;

    private const string Password = "SecurePass123!";
    private const string OrganizerEmail = "jane@example.com";
    private const string ParticipantEmail = "sam@example.com";

    public ApiTests(RaceDayFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClient()
    {
        return _factory.CreateClient();
    }

    private static object CreateOrganizer()
    {
        return new
        {
            firstName = "Jane",
            lastName = "Mokoena",
            email = OrganizerEmail,
            password = Password,
            phoneNumber = "0821111111"
        };
    }

    private static object CreateParticipant()
    {
        return new
        {
            firstName = "Sam",
            lastName = "Nkosi",
            email = ParticipantEmail,
            password = Password,
            dateOfBirth = "2000-01-01"
        };
    }

    private static object CreateEvent()
    {
        return new
        {
            eventName = "Test Race",
            description = "Community running event",
            eventType = "Running",
            eventDate = "2030-10-20T08:00:00Z",
            registrationClosingDate = "2030-10-19T08:00:00Z",
            venue = "Park",
            city = "Pretoria",
            province = "Gauteng",
            routeInformation = "Loop"
        };
    }

    private async Task<HttpClient> LoginAsync(string role)
    {
        var client = CreateClient();

        var email = role switch
        {
            "Organiser" => OrganizerEmail,
            "Participant" => ParticipantEmail,
            _ => throw new ArgumentException(
                "Unsupported user role.",
                nameof(role))
        };

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                email,
                password = Password
            });

        response.EnsureSuccessStatusCode();

        using var json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        var token = json.RootElement
            .GetProperty("token")
            .GetString();

        Assert.False(string.IsNullOrWhiteSpace(token));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    [Fact]
    public async Task Public_events_accessible()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/api/events");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Unauthenticated_create_event_is_rejected()
    {
        using var client = CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/events",
            CreateEvent());

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Registration_and_login_work()
    {
        using var client = CreateClient();

        // These accounts are deliberately seeded by the test factory.
        // Registering them again must return Conflict.
        var organizerResponse = await client.PostAsJsonAsync(
            "/api/auth/register/organizer",
            CreateOrganizer());

        var participantResponse = await client.PostAsJsonAsync(
            "/api/auth/register/participant",
            CreateParticipant());

        Assert.Equal(
            HttpStatusCode.Conflict,
            organizerResponse.StatusCode);

        Assert.Equal(
            HttpStatusCode.Conflict,
            participantResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                email = OrganizerEmail,
                password = Password
            });

        Assert.Equal(
            HttpStatusCode.OK,
            loginResponse.StatusCode);
    }

    [Fact]
    public async Task Duplicate_registration_is_rejected()
    {
        using var client = CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/register/organizer",
            CreateOrganizer());

        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode);
    }

    [Fact]
    public async Task Wrong_password_is_rejected()
    {
        using var client = CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                email = OrganizerEmail,
                password = "WrongPassword123!"
            });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Participant_cannot_create_event()
    {
        using var client = await LoginAsync("Participant");

        var response = await client.PostAsJsonAsync(
            "/api/events",
            CreateEvent());

        Assert.True(
            response.StatusCode == HttpStatusCode.Forbidden,
            $"Expected 403 Forbidden but received " +
            $"{(int)response.StatusCode}. " +
            $"Authentication: {response.Headers.WwwAuthenticate}");
    }

    [Fact]
    public async Task Organizer_can_create_event_and_participant_can_enrol()
    {
        using var organizer = await LoginAsync("Organiser");

        // Step 1: Organizer creates an event.
        var eventResponse = await organizer.PostAsJsonAsync(
            "/api/events",
            CreateEvent());

        Assert.True(
            eventResponse.StatusCode == HttpStatusCode.Created,
            $"Event creation failed: " +
            $"{(int)eventResponse.StatusCode}. " +
            $"Authentication: {eventResponse.Headers.WwwAuthenticate}");

        using var eventJson = JsonDocument.Parse(
            await eventResponse.Content.ReadAsStringAsync());

        var eventId = eventJson.RootElement
            .GetProperty("eventId")
            .GetInt32();

        Assert.True(eventId > 0);

        // Step 2: Organizer creates an event category.
        var categoryResponse = await organizer.PostAsJsonAsync(
            $"/api/events/{eventId}/categories",
            new
            {
                categoryName = "10 km",
                distanceKm = 10,
                entryFee = 100,
                maximumParticipants = 100
            });

        Assert.Equal(
            HttpStatusCode.Created,
            categoryResponse.StatusCode);

        using var categoryJson = JsonDocument.Parse(
            await categoryResponse.Content.ReadAsStringAsync());

        var categoryId = categoryJson.RootElement
            .GetProperty("categoryId")
            .GetInt32();

        Assert.True(categoryId > 0);

        // Step 3: Participant logs in and enrols.
        using var participant = await LoginAsync("Participant");

        var enrolmentResponse = await participant.PostAsJsonAsync(
            "/api/enrolments",
            new
            {
                categoryId
            });

        Assert.Equal(
            HttpStatusCode.Created,
            enrolmentResponse.StatusCode);

        // Step 4: Participant retrieves personal enrolments.
        var myEnrolmentsResponse = await participant.GetAsync(
            "/api/enrolments/my");

        Assert.Equal(
            HttpStatusCode.OK,
            myEnrolmentsResponse.StatusCode);

        var enrolmentsJson =
            await myEnrolmentsResponse.Content.ReadAsStringAsync();

        Assert.Contains(
            $"\"categoryId\":{categoryId}",
            enrolmentsJson);
    }
}

public sealed class RaceDayFactory : WebApplicationFactory<Program>
{
    private const string TestJwtKey =
        "TestOnlyJwtKeyAtLeast32CharactersLong!!";

    private readonly SqliteConnection _connection =
        new("Data Source=:memory:");

    public RaceDayFactory()
    {
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = TestJwtKey,
                    ["Jwt:Issuer"] = "RaceDay.Api",
                    ["Jwt:Audience"] = "RaceDay.Client"
                });
        });

        builder.ConfigureServices(services =>
        {
            // Remove the SQL Server database registration.
            services.RemoveAll<DbContextOptions<RaceDayDbContext>>();
            services.RemoveAll<RaceDayDbContext>();

            // Use SQLite in-memory for integration tests.
            services.AddDbContext<RaceDayDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<RaceDayDbContext>();

        db.Database.EnsureCreated();

        SeedDatabase(db);

        return host;
    }

    private static void SeedDatabase(RaceDayDbContext db)
    {
        if (db.Organizers.Any() || db.Participants.Any())
        {
            return;
        }

        var hasher = new PasswordHasher<object>();

        var organizer = new Organizer
        {
            RoleId = 1,
            FirstName = "Jane",
            LastName = "Mokoena",
            Email = "jane@example.com"
        };

        organizer.PasswordHash = hasher.HashPassword(
            organizer,
            "SecurePass123!");

        var participant = new Participant
        {
            RoleId = 2,
            FirstName = "Sam",
            LastName = "Nkosi",
            Email = "sam@example.com",
            DateOfBirth = new DateOnly(2000, 1, 1)
        };

        participant.PasswordHash = hasher.HashPassword(
            participant,
            "SecurePass123!");

        db.Organizers.Add(organizer);
        db.Participants.Add(participant);

        db.SaveChanges();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
