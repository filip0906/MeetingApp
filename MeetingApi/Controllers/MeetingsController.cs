using Microsoft.AspNetCore.Mvc;
using MeetingApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace MeetingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private static List<Meeting> meetings = new List<Meeting>();

        // Get all meetings
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(meetings);
        }

        // Get meeting by ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var meeting = meetings.FirstOrDefault(m => m.Id == id);
            if (meeting == null)
            {
                return NotFound();
            }
            return Ok(meeting);
        }

        // Create a new meeting
        [HttpPost]
        public IActionResult Create(Meeting meeting)
        {
            meeting.Id = meetings.Count + 1;
            meetings.Add(meeting);
            return CreatedAtAction(nameof(GetById), new { id = meeting.Id }, meeting);
        }

        // Update an existing meeting
        [HttpPut("{id}")]
        public IActionResult Update(int id, Meeting meeting)
        {
            var existingMeeting = meetings.FirstOrDefault(m => m.Id == id);
            if (existingMeeting == null)
            {
                return NotFound();
            }

            existingMeeting.Title = meeting.Title;
            existingMeeting.Date = meeting.Date;
            existingMeeting.Invitees = meeting.Invitees; // Update the invitees list

            return NoContent();
        }

        // Delete a meeting
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var meeting = meetings.FirstOrDefault(m => m.Id == id);
            if (meeting == null)
            {
                return NotFound();
            }

            meetings.Remove(meeting);
            return NoContent();
        }
    }
}
