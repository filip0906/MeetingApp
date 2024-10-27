using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MeetingApi.Models;

namespace MeetingApi.Services
{
    public class GoogleCalendarService
    {
        private readonly CalendarService _calendarService;

        public GoogleCalendarService(string[] scopes, string applicationName, string credentialPath)
        {
            // Inicijalizacija Google Calendar API servisa
            UserCredential credential;

            using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("token.json", true)).Result;
            }

            // Kreiraj CalendarService
            _calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName,
            });
        }

        public async Task<Event> CreateGoogleMeetEventAsync(Meeting meeting)
        {
            var googleEvent = new Event()
            {
                Summary = meeting.Title,
                Location = "Online",
                Description = meeting.Description,
                Start = new EventDateTime()
                {
                    DateTime = DateTime.Parse($"{meeting.Date.ToString("yyyy-MM-dd")}T{meeting.Time}"),
                    TimeZone = "Europe/Belgrade",
                },
                End = new EventDateTime()
                {
                    DateTime = DateTime.Parse($"{meeting.Date.ToString("yyyy-MM-dd")}T{meeting.Time}").AddHours(1),  // Pretpostavka da sastanak traje sat vremena
                    TimeZone = "Europe/Belgrade",
                },
                Attendees = meeting.Invitees.Select(email => new EventAttendee() { Email = email }).ToList(),
                ConferenceData = new ConferenceData()
                {
                    CreateRequest = new CreateConferenceRequest()
                    {
                        RequestId = Guid.NewGuid().ToString(),
                        ConferenceSolutionKey = new ConferenceSolutionKey() { Type = "hangoutsMeet" }
                    }
                },
                Reminders = new Event.RemindersData()
                {
                    UseDefault = false,
                    Overrides = new List<EventReminder>()
                    {
                        new EventReminder() { Method = "email", Minutes = 24 * 60 },  // Podsjetnik 1 dan prije
                        new EventReminder() { Method = "popup", Minutes = 10 },  // Podsjetnik 10 minuta prije
                    }
                }
            };

            var request = _calendarService.Events.Insert(googleEvent, "primary");
            request.ConferenceDataVersion = 1;  // Potrebno za uključivanje konferencijskih podataka u odgovor
            var createdEvent = await request.ExecuteAsync();

            return createdEvent;
        }
    }
}
