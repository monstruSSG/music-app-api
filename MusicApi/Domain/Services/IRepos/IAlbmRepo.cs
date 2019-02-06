using SpotifyApi.Domain.Dtos.ResourceParameters;
using SpotifyApi.Domain.Models;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Services
{
    public interface IAlbmRepo : IRepo<Album, AlbumResourceParameters>
    {
        Task<Album> GetAlbumByNameAsync(string albumName);
        Task<Album> AddTrackToAlbum(int id, Track track);
    }
}
