using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.EntityModels
{
    public class UserAgent
    {
        public int UserAgentId { get; set; }
        public string UserAgentDescription { get; set; }//
        public string SimpleSoftware { get; set; }//
        public string SimpleSubDescription { get; set; } //
        public string Software { get; set; }//
        public string SoftwareName { get; set; } //
        public string OperatingSystem { get; set; }//
        public string OperatingSystemName { get; set; }//   
        public string OperatingSystemVersion { get; set; }//

    }
}
