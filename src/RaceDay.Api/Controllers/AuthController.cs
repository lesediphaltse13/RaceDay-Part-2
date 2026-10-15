using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Services;
namespace RaceDay.Api.Controllers;
[ApiController][Route("api/auth")]
public class AuthController(RaceDayDbContext db,TokenService tokens):ControllerBase {
 private static readonly PasswordHasher<object> Hasher=new();
 private async Task<bool> EmailExists(string email)=>await db.Organizers.AnyAsync(x=>x.Email==email)||await db.Participants.AnyAsync(x=>x.Email==email);
 /// <summary>Register a new organiser account.</summary>
 [HttpPost("register/organizer")][AllowAnonymous]public async Task<IActionResult> RegisterOrganizer(RegisterRequest request){
  var email=request.Email.Trim().ToLowerInvariant();if(await EmailExists(email))return Conflict(new{message="Email already registered"});
  var user=new Organizer{RoleId=1,FirstName=request.FirstName,LastName=request.LastName,Email=email,PhoneNumber=request.PhoneNumber};user.PasswordHash=Hasher.HashPassword(user,request.Password);db.Organizers.Add(user);await db.SaveChangesAsync();return StatusCode(201,new{id=user.OrganiserId,role="Organiser"});
 }
 /// <summary>Register a participant; date of birth is required.</summary>
 [HttpPost("register/participant")][AllowAnonymous]public async Task<IActionResult> RegisterParticipant(RegisterRequest request){
  if(request.DateOfBirth is null||request.DateOfBirth> DateOnly.FromDateTime(DateTime.UtcNow))return BadRequest(new{message="Valid date of birth is required"});
  var email=request.Email.Trim().ToLowerInvariant();if(await EmailExists(email))return Conflict(new{message="Email already registered"});
  var user=new Participant{RoleId=2,FirstName=request.FirstName,LastName=request.LastName,Email=email,PhoneNumber=request.PhoneNumber,DateOfBirth=request.DateOfBirth.Value};user.PasswordHash=Hasher.HashPassword(user,request.Password);db.Participants.Add(user);await db.SaveChangesAsync();return StatusCode(201,new{id=user.ParticipantId,role="Participant"});
 }
 /// <summary>Log in and receive a signed JWT for Swagger Authorize.</summary>
 [HttpPost("login")][AllowAnonymous]public async Task<IActionResult> Login(LoginRequest request){
  var email=request.Email.Trim().ToLowerInvariant();var organiser=await db.Organizers.SingleOrDefaultAsync(x=>x.Email==email);
  if(organiser is not null&&Hasher.VerifyHashedPassword(organiser,organiser.PasswordHash,request.Password)!=PasswordVerificationResult.Failed)return Ok(new{token=tokens.Create(organiser.OrganiserId,"Organiser",email),role="Organiser",expiresInSeconds=7200});
  var participant=await db.Participants.SingleOrDefaultAsync(x=>x.Email==email);
  if(participant is not null&&Hasher.VerifyHashedPassword(participant,participant.PasswordHash,request.Password)!=PasswordVerificationResult.Failed)return Ok(new{token=tokens.Create(participant.ParticipantId,"Participant",email),role="Participant",expiresInSeconds=7200});
  return Unauthorized(new{message="Invalid credentials"});
 }
}
