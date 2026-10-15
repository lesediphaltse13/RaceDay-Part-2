using System.ComponentModel.DataAnnotations;
namespace RaceDay.Api.DTOs;
public record RegisterRequest([Required,MaxLength(50)]string FirstName,[Required,MaxLength(50)]string LastName,[Required,EmailAddress,MaxLength(100)]string Email,[Required,MinLength(8)]string Password,string? PhoneNumber,DateOnly? DateOfBirth);
public record LoginRequest([Required,EmailAddress]string Email,[Required]string Password);
public record ProfileUpdateRequest([Required,MaxLength(50)]string FirstName,[Required,MaxLength(50)]string LastName,string? PhoneNumber);
public record EventRequest([Required,MaxLength(120)]string EventName,string? Description,[Required]string EventType,DateTime EventDate,DateTime RegistrationClosingDate,[Required]string Venue,[Required]string City,[Required]string Province,string? RouteInformation);
public record CategoryRequest([Required]string CategoryName,decimal DistanceKm,decimal EntryFee,int? MaximumParticipants);
public record EnrolRequest(int CategoryId);
public record ResultRequest(int EnrolmentId,TimeOnly? FinishTime,int? Position,string Status);
