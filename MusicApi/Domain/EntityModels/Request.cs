using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.EntityModels
{
    //class used for tracking all requests made to this api
    public class Request
    {
        public int RequestId { get; set; }
        public string Destination { get; set; }
        public string Method { get; set; }
        public string Source { get; set; }
        public DateTime DateTime { get; private set; } = DateTime.Now;
        public UserAgent UserAgent { get; set; }

    }
}
