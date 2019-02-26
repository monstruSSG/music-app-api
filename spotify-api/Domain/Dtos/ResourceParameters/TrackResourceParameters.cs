using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Dtos.ResourceParameters
{
    public class TrackResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// If set returnes Tracks with this Name
        /// </summary>
        public string Name { get; set; }
    }
}
