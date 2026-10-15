using System.Security.Claims;
namespace RaceDay.Api.Services;
public static class IdentityHelpers { public static int UserId(this ClaimsPrincipal user)=>int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier),out var id)?id:0; }
