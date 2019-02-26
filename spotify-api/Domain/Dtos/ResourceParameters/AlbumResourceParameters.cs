using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Dtos.ResourceParameters
{
    public class AlbumResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// If set returnes Albums with this type
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// If set returnes Albums with this name
        /// </summary>
        public string Name { get; set; }
    }
}
