using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.EntityModels
{
    public class PlaylistTrack
    {
        public int PlaylistTrackId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Href { get; set; }

        [StringLength(100)]
        public string PreviewUrl { get; set; }

    }
}
