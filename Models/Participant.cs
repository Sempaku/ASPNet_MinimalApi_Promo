namespace Promo_2022_API.Models
{
    public class Participant
    {
        public int Id { get; private set; }
        public string Name { get; set; } = null!;
        public static int globalParticipantID;

        public Participant()
        {
            Id = Interlocked.Increment(ref globalParticipantID);
        }
    }
}
