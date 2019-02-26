using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using SpotifyApi.Domain.EntityModels;
using SpotifyApi.Domain.Logic;

namespace SpotifyApi.Domain.Services
{
    public class PlaylistArtistRepo : IPlaylistArtist
    {

        private readonly DataContext _context;

        public PlaylistArtistRepo(DataContext context)
        {
            _context = context;
        }

        public void Add(PlaylistArtist t)
        {
            _context.PlaylistArtists.Add(t);
        }

        public void Delete(PlaylistArtist t)
        {
            _context.PlaylistArtists.Remove(t);
        }

        public async Task<List<PlaylistArtist>> GetAllAsync()
        {
            return await _context.PlaylistArtists
                .Include(t => t.Tracks)
                .ToListAsync();
        }

        public PagedList<PlaylistArtist> GetAllPagination(PlaylistArtistResourceParameters resourceParams)
        {
            var collectionBeforePaging = _context.PlaylistArtists
                .Include(t => t.Tracks)
                .OrderBy(a => a.Name)
                .AsQueryable();


            //filter by userName
            if (!string.IsNullOrEmpty(resourceParams.UserName))
            {
                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.UserName == resourceParams.UserName);
            }



            //if name filter exists
            if (!string.IsNullOrEmpty(resourceParams.Name))
            {
                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.Name == resourceParams.Name);
            }

            //searh if exists
            if (!string.IsNullOrEmpty(resourceParams.SearchQuery))
            {
                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.Name.Contains(resourceParams.SearchQuery));
            }

            return PagedList<PlaylistArtist>.Create(collectionBeforePaging, resourceParams.PageNumber, resourceParams.PageSize);
        }

        public async Task<PlaylistArtist> GetByIdAsync(int id)
        {
            var artist = await _context.PlaylistArtists.Include(t => t.Tracks).FirstOrDefaultAsync(art => art.PlaylistArtistId == id);

            return artist;
        }

        public bool GetByName(string name, string username)
        {

            if (_context.PlaylistArtists.FirstOrDefault(a => a.Name == name && a.UserName == username) != null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> SaveChangesAsync()
        {
            //returntrue if 1 or more entities were changed
            return (await _context.SaveChangesAsync() > 0);
        }

        public void Update(int id, PlaylistArtist t)
        {
            var playlistArtist = _context.PlaylistArtists.Include(tr => tr.Tracks).FirstOrDefault(art => art.PlaylistArtistId == id);

            playlistArtist.ImgUri = t.ImgUri;
            playlistArtist.Name = t.Name;
            playlistArtist.PlaylistArtistId = t.PlaylistArtistId;
            playlistArtist.Tracks = t.Tracks;
            playlistArtist.Uri = t.Uri;

            _context.PlaylistArtists.Update(playlistArtist);
        }
    }
}
