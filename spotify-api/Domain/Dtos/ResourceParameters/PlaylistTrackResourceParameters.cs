using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Dtos.ResourceParameters
{
    public class PlaylistTrackResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// If set returnes Tracks with this name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// If set returnes Tracks with this userName
        /// </summary>
        public string UserName { get; set; }
    }
}
