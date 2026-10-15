using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Services;
namespace RaceDay.Api.Controllers;
[ApiController][Route("api/events")]
public class EventsController(RaceDayDbContext db):ControllerBase {
 private static object ViewEvent(RaceEvent x)=>new{x.EventId,x.OrganiserId,x.EventName,x.Description,x.EventType,x.EventDate,x.RegistrationClosingDate,x.Venue,x.City,x.Province,x.RouteInformation,x.CreatedAt};
 private static string? Error(EventRequest r)=>!new[]{"Running","Walking","Cycling"}.Contains(r.EventType,StringComparer.OrdinalIgnoreCase)?"EventType must be Running, Walking or Cycling":r.RegistrationClosingDate>=r.EventDate?"Registration must close before event date":null;
 /// <summary>List all events; optionally filter by type or city.</summary>
 [HttpGet][AllowAnonymous]public async Task<IActionResult> All([FromQuery]string? type,[FromQuery]string? city){var q=db.Events.AsNoTracking().AsQueryable();if(!string.IsNullOrWhiteSpace(type))q=q.Where(x=>x.EventType==type);if(!string.IsNullOrWhiteSpace(city))q=q.Where(x=>x.City==city);var list=await q.OrderBy(x=>x.EventDate).ToListAsync();return Ok(list.Select(ViewEvent));}
 /// <summary>View an event by ID.</summary>
 [HttpGet("{id:int}")][AllowAnonymous]public async Task<IActionResult> One(int id){var x=await db.Events.FindAsync(id);return x is null?NotFound():Ok(ViewEvent(x));}
 /// <summary>List events owned by the logged-in organiser.</summary>
 [HttpGet("/api/organizers/events")][Authorize(Roles="Organiser")]public async Task<IActionResult> Mine(){var id=User.UserId();var list=await db.Events.Where(x=>x.OrganiserId==id).OrderBy(x=>x.EventDate).ToListAsync();return Ok(list.Select(ViewEvent));}
 /// <summary>Create an event as an organiser.</summary>
 [HttpPost][Authorize(Roles="Organiser")]public async Task<IActionResult> Create(EventRequest r){var error=Error(r);if(error is not null)return BadRequest(new{message=error});var x=new RaceEvent{OrganiserId=User.UserId()};Apply(x,r);db.Events.Add(x);await db.SaveChangesAsync();return CreatedAtAction(nameof(One),new{id=x.EventId},ViewEvent(x));}
 /// <summary>Update an event you own.</summary>
 [HttpPut("{id:int}")][Authorize(Roles="Organiser")]public async Task<IActionResult> Update(int id,EventRequest r){var x=await db.Events.FindAsync(id);if(x is null)return NotFound();if(x.OrganiserId!=User.UserId())return Forbid();var error=Error(r);if(error is not null)return BadRequest(new{message=error});Apply(x,r);await db.SaveChangesAsync();return NoContent();}
 /// <summary>Delete an event you own; rejects events with categories.</summary>
 [HttpDelete("{id:int}")][Authorize(Roles="Organiser")]public async Task<IActionResult> Delete(int id){var x=await db.Events.FindAsync(id);if(x is null)return NotFound();if(x.OrganiserId!=User.UserId())return Forbid();if(await db.EventCategories.AnyAsync(c=>c.EventId==id))return Conflict(new{message="Remove event categories first"});db.Events.Remove(x);await db.SaveChangesAsync();return NoContent();}
 private static void Apply(RaceEvent x,EventRequest r){x.EventName=r.EventName;x.Description=r.Description;x.EventType=r.EventType;x.EventDate=r.EventDate;x.RegistrationClosingDate=r.RegistrationClosingDate;x.Venue=r.Venue;x.City=r.City;x.Province=r.Province;x.RouteInformation=r.RouteInformation;}
}
