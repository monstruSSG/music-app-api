using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpotifyApi.Domain.Dtos
{
    public class PlaylistArtistToCreateDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Uri { get; set; }

        [StringLength(100)]
        public string ImgUri { get; set; }

        public IEnumerable<PlaylistTrackToCreateDto> Tracks { get; set; }
    }
}
