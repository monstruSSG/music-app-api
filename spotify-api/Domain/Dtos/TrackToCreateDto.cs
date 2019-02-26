using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SpotifyApi.Domain.Dtos
{
    public class TrackToCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Href { get; set; }

        [StringLength(500)]
        public string PreviewUrl { get; set; }

        public IEnumerable<ArtistToCreateDto> Artists { get; set; }

    }
}
