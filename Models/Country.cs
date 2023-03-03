using System.ComponentModel.DataAnnotations;

namespace gold_medal_backend.Models
{
    public class Country
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int GoldMedalCount { get; set; } = 0;
        public int SilverMedalCount { get; set; } = 0;
        public int BronzeMedalCount { get; set; } = 0;
    }
}