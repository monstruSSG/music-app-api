using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Dtos.ResourceParameters
{
    public class RequestResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// If set returnes Requests with this Method
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// If set returnes Request with this destination
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// If set returnes Requests with this Source
        /// </summary>
        public string Source { get; set; }
    }
}
