using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Services;
namespace RaceDay.Api.Controllers;
[ApiController][Authorize]public class ResultsController(RaceDayDbContext db):ControllerBase {
 private static object ViewResult(RaceResult x)=>new{x.ResultId,x.EnrolmentId,x.FinishTime,x.Position,x.Status,x.RecordedAt};
 private async Task<Enrolment?> GetEnrolment(int id)=>await db.Enrolments.Include(x=>x.Category).ThenInclude(x=>x!.Event).SingleOrDefaultAsync(x=>x.EnrolmentId==id);
 /// <summary>Capture a participant's race result as the event organiser.</summary>
 [HttpPost("api/enrolments/{enrolmentId:int}/result")][Authorize(Roles="Organiser")]public async Task<IActionResult> Create(int enrolmentId,ResultRequest r){if(r.EnrolmentId!=enrolmentId)return BadRequest(new{message="Enrolment IDs do not match"});var en=await GetEnrolment(enrolmentId);if(en is null)return NotFound();if(en.Category!.Event!.OrganiserId!=User.UserId())return Forbid();if(en.Status!="Confirmed")return Conflict(new{message="Enrolment is not confirmed"});if(await db.Results.AnyAsync(x=>x.EnrolmentId==enrolmentId))return Conflict(new{message="Result already exists"});if(!Valid(r))return BadRequest(new{message="Invalid result"});var x=new RaceResult{EnrolmentId=enrolmentId,FinishTime=r.FinishTime,Position=r.Position,Status=r.Status};db.Results.Add(x);await db.SaveChangesAsync();return StatusCode(201,ViewResult(x));}
 /// <summary>View your results as a participant.</summary>
 [HttpGet("api/results/my")][Authorize(Roles="Participant")]public async Task<IActionResult> Mine(){var id=User.UserId();var list=await db.Results.Where(x=>x.Enrolment!.ParticipantId==id).ToListAsync();return Ok(list.Select(ViewResult));}
 /// <summary>View a result if you are its participant or event organiser.</summary>
 [HttpGet("api/results/{id:int}")]public async Task<IActionResult> One(int id){var x=await db.Results.Include(x=>x.Enrolment).ThenInclude(x=>x!.Category).ThenInclude(x=>x!.Event).SingleOrDefaultAsync(x=>x.ResultId==id);if(x is null)return NotFound();if(!Owns(x))return Forbid();return Ok(ViewResult(x));}
 /// <summary>View results for an event you organise.</summary>
 [HttpGet("api/events/{eventId:int}/results")][Authorize(Roles="Organiser")]public async Task<IActionResult> ForEvent(int eventId){var ev=await db.Events.FindAsync(eventId);if(ev is null)return NotFound();if(ev.OrganiserId!=User.UserId())return Forbid();var list=await db.Results.Where(x=>x.Enrolment!.Category!.EventId==eventId).ToListAsync();return Ok(list.Select(ViewResult));}
 /// <summary>Update a result for an event you organise.</summary>
 [HttpPut("api/results/{id:int}")][Authorize(Roles="Organiser")]public async Task<IActionResult> Update(int id,ResultRequest r){var x=await db.Results.Include(x=>x.Enrolment).ThenInclude(x=>x!.Category).ThenInclude(x=>x!.Event).SingleOrDefaultAsync(x=>x.ResultId==id);if(x is null)return NotFound();if(!Owns(x))return Forbid();if(x.EnrolmentId!=r.EnrolmentId||!Valid(r))return BadRequest();x.FinishTime=r.FinishTime;x.Position=r.Position;x.Status=r.Status;x.RecordedAt=DateTime.UtcNow;await db.SaveChangesAsync();return NoContent();}
 /// <summary>Delete a result for an event you organise.</summary>
 [HttpDelete("api/results/{id:int}")][Authorize(Roles="Organiser")]public async Task<IActionResult> Delete(int id){var x=await db.Results.Include(x=>x.Enrolment).ThenInclude(x=>x!.Category).ThenInclude(x=>x!.Event).SingleOrDefaultAsync(x=>x.ResultId==id);if(x is null)return NotFound();if(!Owns(x))return Forbid();db.Results.Remove(x);await db.SaveChangesAsync();return NoContent();}
 private bool Owns(RaceResult x)=>User.IsInRole("Participant")&&x.Enrolment!.ParticipantId==User.UserId()||User.IsInRole("Organiser")&&x.Enrolment!.Category!.Event!.OrganiserId==User.UserId();
 private static bool Valid(ResultRequest r)=>!string.IsNullOrWhiteSpace(r.Status)&&r.Position is null or >0;
}
