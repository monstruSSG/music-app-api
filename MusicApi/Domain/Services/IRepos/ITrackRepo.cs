using SpotifyApi.Domain.Dtos.ResourceParameters;
using SpotifyApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Services
{
    public interface ITrackRepo : IRepo<Track, TrackResourceParameters>
    {
        Task<Track> AddArtistToTrack(int id, Artist artist);
    }
}
