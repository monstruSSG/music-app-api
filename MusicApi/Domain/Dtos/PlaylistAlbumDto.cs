using SpotifyApi.Domain.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Dtos
{
    public class PlaylistAlbumDto : LinkedResourceBaseDto
    {
        public int PlaylistAlbumId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string ImgUri { get; set; }

        public IEnumerable<PlaylistTrackDto> Tracks { get; set; }
    }
}
