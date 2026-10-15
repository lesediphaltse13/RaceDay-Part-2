using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Services;
namespace RaceDay.Api.Controllers;
[ApiController][Authorize]public class ProfilesController(RaceDayDbContext db):ControllerBase {
 /// <summary>View the authenticated organiser's profile.</summary>
 [HttpGet("api/organizers/profile")][Authorize(Roles="Organiser")]public async Task<IActionResult> OrganizerProfile(){var id=User.UserId();var x=await db.Organizers.Where(x=>x.OrganiserId==id).Select(x=>new{x.OrganiserId,x.FirstName,x.LastName,x.Email,x.PhoneNumber,x.CreatedAt}).SingleOrDefaultAsync();return x is null?NotFound():Ok(x);}
 /// <summary>Update the authenticated organiser's profile.</summary>
 [HttpPut("api/organizers/profile")][Authorize(Roles="Organiser")]public async Task<IActionResult> UpdateOrganizer(ProfileUpdateRequest r){var x=await db.Organizers.FindAsync(User.UserId());if(x is null)return NotFound();x.FirstName=r.FirstName;x.LastName=r.LastName;x.PhoneNumber=r.PhoneNumber;await db.SaveChangesAsync();return NoContent();}
 /// <summary>View the authenticated participant's profile.</summary>
 [HttpGet("api/participants/profile")][Authorize(Roles="Participant")]public async Task<IActionResult> ParticipantProfile(){var id=User.UserId();var x=await db.Participants.Where(x=>x.ParticipantId==id).Select(x=>new{x.ParticipantId,x.FirstName,x.LastName,x.Email,x.PhoneNumber,x.DateOfBirth,x.CreatedAt}).SingleOrDefaultAsync();return x is null?NotFound():Ok(x);}
 /// <summary>Update the authenticated participant's profile.</summary>
 [HttpPut("api/participants/profile")][Authorize(Roles="Participant")]public async Task<IActionResult> UpdateParticipant(ProfileUpdateRequest r){var x=await db.Participants.FindAsync(User.UserId());if(x is null)return NotFound();x.FirstName=r.FirstName;x.LastName=r.LastName;x.PhoneNumber=r.PhoneNumber;await db.SaveChangesAsync();return NoContent();}
}
