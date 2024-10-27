
namespace MeetingApi.Models
{
    public class Invitee
    {
        public int Id { get; set; }  // Primarni ključ
        public string Email { get; set; }  // Email pozvane osobe
        public InvitationStatus Status { get; set; } = InvitationStatus.Pending;

        public static implicit operator string(Invitee v)
        {
            throw new NotImplementedException();
        }
    }

    public enum InvitationStatus
    {
        Accepted,
        Declined,
        Pending
    }
}
