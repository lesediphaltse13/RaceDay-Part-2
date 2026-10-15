using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Services;
namespace RaceDay.Api.Controllers;
[ApiController]public class CategoriesController(RaceDayDbContext db):ControllerBase {
 private static object ViewCategory(EventCategory x)=>new{x.CategoryId,x.EventId,x.CategoryName,x.DistanceKm,x.EntryFee,x.MaximumParticipants};
 private static string? Error(CategoryRequest r)=>r.DistanceKm<=0||r.EntryFee<0||r.MaximumParticipants<=0?"Distance must be positive, fee non-negative and capacity positive":null;
 /// <summary>List all categories for an event.</summary>
 [HttpGet("api/events/{eventId:int}/categories")][AllowAnonymous]public async Task<IActionResult> All(int eventId){if(!await db.Events.AnyAsync(x=>x.EventId==eventId))return NotFound();var list=await db.EventCategories.Where(x=>x.EventId==eventId).ToListAsync();return Ok(list.Select(ViewCategory));}
 /// <summary>View a category.</summary>
 [HttpGet("api/categories/{id:int}")][AllowAnonymous]public async Task<IActionResult> One(int id){var x=await db.EventCategories.FindAsync(id);return x is null?NotFound():Ok(ViewCategory(x));}
 /// <summary>Add a category to your event.</summary>
 [HttpPost("api/events/{eventId:int}/categories")][Authorize(Roles="Organiser")]public async Task<IActionResult> Create(int eventId,CategoryRequest r){var ev=await db.Events.FindAsync(eventId);if(ev is null)return NotFound();if(ev.OrganiserId!=User.UserId())return Forbid();var error=Error(r);if(error is not null)return BadRequest(new{message=error});var x=new EventCategory{EventId=eventId};Apply(x,r);db.EventCategories.Add(x);await db.SaveChangesAsync();return CreatedAtAction(nameof(One),new{id=x.CategoryId},ViewCategory(x));}
 /// <summary>Update a category belonging to your event.</summary>
 [HttpPut("api/categories/{id:int}")][Authorize(Roles="Organiser")]public async Task<IActionResult> Update(int id,CategoryRequest r){var x=await db.EventCategories.Include(x=>x.Event).SingleOrDefaultAsync(x=>x.CategoryId==id);if(x is null)return NotFound();if(x.Event!.OrganiserId!=User.UserId())return Forbid();var error=Error(r);if(error is not null)return BadRequest(new{message=error});Apply(x,r);await db.SaveChangesAsync();return NoContent();}
 /// <summary>Delete a category without enrolments.</summary>
 [HttpDelete("api/categories/{id:int}")][Authorize(Roles="Organiser")]public async Task<IActionResult> Delete(int id){var x=await db.EventCategories.Include(x=>x.Event).SingleOrDefaultAsync(x=>x.CategoryId==id);if(x is null)return NotFound();if(x.Event!.OrganiserId!=User.UserId())return Forbid();if(await db.Enrolments.AnyAsync(e=>e.CategoryId==id))return Conflict(new{message="Category has enrolments"});db.EventCategories.Remove(x);await db.SaveChangesAsync();return NoContent();}
 private static void Apply(EventCategory x,CategoryRequest r){x.CategoryName=r.CategoryName;x.DistanceKm=r.DistanceKm;x.EntryFee=r.EntryFee;x.MaximumParticipants=r.MaximumParticipants;}
}
