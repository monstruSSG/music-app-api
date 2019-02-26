using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Models
{
    public class Track
    {
        public int TrackId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Href { get; set; }

        [StringLength(500)]
        public string PreviewUrl { get; set; }

        public ICollection<Artist> Artists { get; set; }

    }
}
