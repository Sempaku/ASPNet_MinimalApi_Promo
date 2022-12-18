using System.ComponentModel.DataAnnotations;

namespace Promo_2022_API.Models
{
    public class Promo
    {
        public int? Id { get; set; } = 0;
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<Prize>? Prizes { get; set; } = new List<Prize> { };
        public List<Participant>? Participants { get; set; } = new List<Participant> { };
        public static int globalPromoID;
        public Promo()
        {
            Id = Interlocked.Increment(ref globalPromoID);
        }
    }
}
