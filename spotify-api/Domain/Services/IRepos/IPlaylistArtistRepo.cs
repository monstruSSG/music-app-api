﻿using SpotifyApi.Domain.Dtos.ResourceParameters;
using SpotifyApi.Domain.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Services
{
    public interface IPlaylistArtist : IRepo<PlaylistArtist, PlaylistArtistResourceParameters>
    {
        bool GetByName(string name, string username);
    }
}
