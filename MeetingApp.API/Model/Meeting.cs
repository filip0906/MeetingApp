namespace MeetingApp.API.Model
{
    public class Meeting
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public List<string> Participants { get; set; }
    }
}
