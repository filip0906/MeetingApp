using MeetingApp.API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeetingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeetingsController : ControllerBase
    {
        private static List<Meeting> meetings = new List<Meeting>();

        [HttpGet]
        public IEnumerable<Meeting> GetMeetings()
        {
            return meetings;
        }

        [HttpPost]
        public IActionResult CreateMeeting(Meeting meeting)
        {
            meeting.Id = meetings.Count > 0 ? meetings.Max(m => m.Id) + 1 : 1;
            meetings.Add(meeting);
            return Ok(meeting);
        }
    }

}
