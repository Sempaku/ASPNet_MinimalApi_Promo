namespace Promo_2022_API.Models
{
    public class Prize
    {
        public int Id { get; private set; }
        public string Description { get; set; } = string.Empty;

        public static int globalPrizeID;

        public Prize()
        {
            Id = Interlocked.Increment(ref globalPrizeID);
        }
    }
}
