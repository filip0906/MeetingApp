using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using MeetingApp.API.Model;

namespace MeetingApp.Client.Services
{
    public class MeetingService
    {
        private readonly HttpClient _httpClient;

        public MeetingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Meeting>> GetMeetings()
        {
            return await _httpClient.GetFromJsonAsync<List<Meeting>>("api/meetings");
        }

        public async Task<Meeting> CreateMeeting(Meeting meeting)
        {
            var response = await _httpClient.PostAsJsonAsync("api/meetings", meeting);
            return await response.Content.ReadFromJsonAsync<Meeting>();
        }
    }
}
