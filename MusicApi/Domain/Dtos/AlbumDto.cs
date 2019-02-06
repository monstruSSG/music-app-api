using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Dtos
{
    public class AlbumDto : LinkedResourceBaseDto
    {
        public int AlbumId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        [StringLength(100)]
        public string ImgUri { get; set; }

        public IEnumerable<TrackDto> Tracks { get; set; }


    }
}
