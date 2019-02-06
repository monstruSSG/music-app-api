using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpotifyApi.Domain.Dtos
{
    public class TrackDto : LinkedResourceBaseDto
    {
        public int TrackId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Href { get; set; }

        [StringLength(100)]
        public string PreviewUrl { get; set; }

        public IEnumerable<ArtistDto> Artists { get; set; }

    }
}
