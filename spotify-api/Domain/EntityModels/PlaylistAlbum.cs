using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SpotifyApi.Domain.EntityModels
{
    public class PlaylistAlbum
    {
        public int PlaylistAlbumId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        [StringLength(100)]
        public string ImgUri { get; set; }

        public ICollection<PlaylistTrack> Tracks { get; set; }

    }
}
