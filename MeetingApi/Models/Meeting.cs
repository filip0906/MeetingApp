namespace MeetingApi.Models
{
    public class Meeting
    {
        public int Id { get; set; } // Pretpostavka je da koristite bazu podataka
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public List<string> Invitees { get; set; } = new List<string>();
    }

}
