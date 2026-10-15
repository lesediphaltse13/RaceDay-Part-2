using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Services;
namespace RaceDay.Api.Controllers;
[ApiController][Authorize]public class EnrolmentsController(RaceDayDbContext db):ControllerBase {
 private static object ViewEnrolment(Enrolment x)=>new{x.EnrolmentId,x.ParticipantId,x.CategoryId,x.EnrolmentDate,x.Status,x.RaceNumber};
 /// <summary>Enrol as a participant in an event category.</summary>
 [HttpPost("api/enrolments")][Authorize(Roles="Participant")]public async Task<IActionResult> Create(EnrolRequest r){var category=await db.EventCategories.Include(x=>x.Event).SingleOrDefaultAsync(x=>x.CategoryId==r.CategoryId);if(category is null)return NotFound();if(category.Event!.RegistrationClosingDate<DateTime.UtcNow)return BadRequest(new{message="Registration has closed"});var id=User.UserId();if(await db.Enrolments.AnyAsync(x=>x.ParticipantId==id&&x.CategoryId==r.CategoryId))return Conflict(new{message="Already enrolled"});if(category.MaximumParticipants.HasValue&&await db.Enrolments.CountAsync(x=>x.CategoryId==r.CategoryId&&x.Status=="Confirmed")>=category.MaximumParticipants.Value)return Conflict(new{message="Category is full"});var x=new Enrolment{ParticipantId=id,CategoryId=r.CategoryId};db.Enrolments.Add(x);await db.SaveChangesAsync();return StatusCode(201,ViewEnrolment(x));}
 /// <summary>View your own enrolments.</summary>
 [HttpGet("api/enrolments/my")][Authorize(Roles="Participant")]public async Task<IActionResult> Mine(){var id=User.UserId();return Ok((await db.Enrolments.Where(x=>x.ParticipantId==id).ToListAsync()).Select(ViewEnrolment));}
 /// <summary>View an enrolment if it is yours or belongs to an event you organise.</summary>
 [HttpGet("api/enrolments/{id:int}")]public async Task<IActionResult> One(int id){var x=await db.Enrolments.Include(x=>x.Category).ThenInclude(x=>x!.Event).SingleOrDefaultAsync(x=>x.EnrolmentId==id);if(x is null)return NotFound();if(User.IsInRole("Participant")&&x.ParticipantId!=User.UserId()||User.IsInRole("Organiser")&&x.Category!.Event!.OrganiserId!=User.UserId())return Forbid();return Ok(ViewEnrolment(x));}
 /// <summary>Cancel your enrolment if no result has been recorded.</summary>
 [HttpDelete("api/enrolments/{id:int}")][Authorize(Roles="Participant")]public async Task<IActionResult> Cancel(int id){var x=await db.Enrolments.FindAsync(id);if(x is null)return NotFound();if(x.ParticipantId!=User.UserId())return Forbid();if(await db.Results.AnyAsync(r=>r.EnrolmentId==id))return Conflict(new{message="Results already recorded"});x.Status="Cancelled";await db.SaveChangesAsync();return NoContent();}
 /// <summary>View enrolments for an event you organise.</summary>
 [HttpGet("api/events/{eventId:int}/enrolments")][Authorize(Roles="Organiser")]public async Task<IActionResult> ForEvent(int eventId){var ev=await db.Events.FindAsync(eventId);if(ev is null)return NotFound();if(ev.OrganiserId!=User.UserId())return Forbid();var list=await db.Enrolments.Where(x=>x.Category!.EventId==eventId).ToListAsync();return Ok(list.Select(ViewEnrolment));}
}
