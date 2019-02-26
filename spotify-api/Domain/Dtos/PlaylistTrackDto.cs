using System.ComponentModel.DataAnnotations;


namespace SpotifyApi.Domain.Dtos
{
    public class PlaylistTrackDto : LinkedResourceBaseDto
    {
        public int PlaylistTrackId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Href { get; set; }

        [StringLength(500)]
        public string PreviewUrl { get; set; }
    }
}
