using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Dtos
{
    public class RequestDto : LinkedResourceBaseDto
    {
        public int RequestId { get; set; }
        public string Destination { get; set; }
        public string Method { get; set; }
        public string Source { get; set; }
        public DateTime DateTime { get; set; } 
        public UserAgentDto UserAgent { get; set; }
    }
}
