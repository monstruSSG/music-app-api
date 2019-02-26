using SpotifyApi.Domain.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Logic.AuxServicies.IAuxServicies
{
    public interface IAuxUserAgentService
    {
        Task<UserAgent> ParseUserAgentData(string userAgentData);
    }
}
