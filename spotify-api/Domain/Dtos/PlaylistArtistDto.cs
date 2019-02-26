using SpotifyApi.Domain.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Dtos
{
    public class PlaylistArtistDto : LinkedResourceBaseDto
    {
        public int PlaylistArtistId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Uri { get; set; }

        [StringLength(100)]
        public string ImgUri { get; set; }

        public IEnumerable<PlaylistTrackDto> Tracks { get; set; }
    }
}
