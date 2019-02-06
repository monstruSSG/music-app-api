using SpotifyApi.Domain.Dtos.ResourceParameters;
using SpotifyApi.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Services
{
    public interface IArtistRepo : IRepo<Artist, ArtistResourceParameters>
    {
    }
}
