using System.ComponentModel.DataAnnotations;

namespace gold_medal_backend.Authentication
{
    public class UserTokenDTO
    {
      [Required]
      public string Token { get; set; }
      [Required]
      public string Expiration { get; set; }
    }
}