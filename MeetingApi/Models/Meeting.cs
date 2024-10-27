namespace MeetingApi.Models
{
    public class Meeting
    {
        public int Id { get; set; } // Pretpostavka je da koristite bazu podataka
        public string Title { get; set; }
        public string Description { get; set; } // Novi atribut za opis sastanka
        public DateTime Date { get; set; }
        public string Time { get; set; } // Promjena iz TimeSpan u string
        public string OrganizerEmail { get; set; } // Novi atribut za e-mail organizatora
        public List<Invitee> Invitees { get; set; } = new List<Invitee>();
    }
}
