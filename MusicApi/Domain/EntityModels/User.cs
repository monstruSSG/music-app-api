using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace SpotifyApi.Domain.Models
{
    public class User : IdentityUser<int>
    {
        [Required]
        public string Password { get; set; }

        [StringLength(100)]
        public string Href { get; set; }


    }
}
