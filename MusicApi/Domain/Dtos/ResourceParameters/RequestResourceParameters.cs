using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Dtos.ResourceParameters
{
    public class RequestResourceParameters : BaseResourceParameters
    {
        public string Method { get; set; }
        public string Destination { get; set; }
        public string Source { get; set; }
    }
}
