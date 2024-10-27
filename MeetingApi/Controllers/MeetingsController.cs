using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetingApi.Models;
using MeetingApi.Services;
using MeetingApi.Data;

namespace MeetingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public MeetingsController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var meetings = await _context.Meetings.Include(m => m.Invitees).ToListAsync();
            return Ok(meetings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var meeting = await _context.Meetings.Include(m => m.Invitees).FirstOrDefaultAsync(m => m.Id == id);
            if (meeting == null)
            {
                return NotFound();
            }
            return Ok(meeting);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Meeting meeting)
        {
            // Dodavanje sastanka u bazu
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            // Slanje e-mail pozivnica
            await SendEmailInvitations(meeting);

            return CreatedAtAction(nameof(GetById), new { id = meeting.Id }, meeting);
        }

        [HttpGet("accept-invitation/{meetingId}/{inviteeEmail}")]
        public async Task<IActionResult> AcceptInvitation(int meetingId, string inviteeEmail)
        {
            var meeting = await _context.Meetings.Include(m => m.Invitees).FirstOrDefaultAsync(m => m.Id == meetingId);
            if (meeting == null)
            {
                return NotFound();
            }

            var invitee = meeting.Invitees.FirstOrDefault(i => i.Email == inviteeEmail);
            if (invitee == null)
            {
                return NotFound();
            }

            invitee.Status = InvitationStatus.Accepted;
            await _context.SaveChangesAsync();
            return Ok("Dolazak je potvrđen.");
        }

        [HttpGet("decline-invitation/{meetingId}/{inviteeEmail}")]
        public async Task<IActionResult> DeclineInvitation(int meetingId, string inviteeEmail)
        {
            var meeting = await _context.Meetings.Include(m => m.Invitees).FirstOrDefaultAsync(m => m.Id == meetingId);
            if (meeting == null)
            {
                return NotFound();
            }

            var invitee = meeting.Invitees.FirstOrDefault(i => i.Email == inviteeEmail);
            if (invitee == null)
            {
                return NotFound();
            }

            invitee.Status = InvitationStatus.Declined;
            await _context.SaveChangesAsync();
            return Ok("Poziv je odbijen.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Meeting meeting)
        {
            var existingMeeting = await _context.Meetings.Include(m => m.Invitees).FirstOrDefaultAsync(m => m.Id == id);
            if (existingMeeting == null)
            {
                return NotFound();
            }

            existingMeeting.Title = meeting.Title;
            existingMeeting.Description = meeting.Description;
            existingMeeting.Date = meeting.Date;
            existingMeeting.Time = meeting.Time;
            existingMeeting.Invitees = meeting.Invitees;

            await _context.SaveChangesAsync();

            // Ponovno slanje e-mail pozivnica nakon izmene sastanka
            await SendEmailInvitations(existingMeeting);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var meeting = await _context.Meetings.Include(m => m.Invitees).FirstOrDefaultAsync(m => m.Id == id);
            if (meeting == null)
            {
                return NotFound();
            }

            _context.Meetings.Remove(meeting);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task SendEmailInvitations(Meeting meeting)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/"; // Generiraj bazni URL iz HttpRequest objekta

            foreach (var invitee in meeting.Invitees)
            {
                var acceptUrl = $"{baseUrl}api/meetings/accept-invitation/{meeting.Id}/{invitee.Email}";
                var declineUrl = $"{baseUrl}api/meetings/decline-invitation/{meeting.Id}/{invitee.Email}";

                var emailBody = $"You are invited to the meeting titled \"{meeting.Title}\" on {meeting.Date.ToShortDateString()} at {meeting.Time}.<br/>" +
                                $"Description: {meeting.Description}<br/>" +
                                $"<a href='{acceptUrl}'>Accept Invitation</a><br/>" +
                                $"<a href='{declineUrl}'>Decline Invitation</a>";

                try
                {
                    await _emailService.SendEmailAsync(invitee.Email, "Meeting Invitation", emailBody, meeting);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email to {invitee.Email}: {ex.Message}");
                }
            }
        }
    }
}
