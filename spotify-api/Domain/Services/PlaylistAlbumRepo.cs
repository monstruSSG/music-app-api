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
    public class PlaylistAlbumRepo : IPlaylistAlbumRepo
    {

        private readonly DataContext _context;

        public PlaylistAlbumRepo(DataContext context)
        {
            _context = context;
        }

        public void Add(PlaylistAlbum t)
        {
            _context.PlaylistAlbums.Add(t);
        }

        public void Delete(PlaylistAlbum t)
        {
            _context.PlaylistAlbums.Remove(t);
        }

        public async Task<List<PlaylistAlbum>> GetAllAsync()
        {
            return await _context.PlaylistAlbums
                .Include(t => t.Tracks)
                .ToListAsync();
        }

        public PagedList<PlaylistAlbum> GetAllPagination(PlaylistAlbumResourceParameters resourceParams)
        {
            var collectionBeforePaging = _context.PlaylistAlbums
                .Include(t => t.Tracks)
                .OrderBy(t => t.UserName)
                .AsQueryable();

            //filter by userName
            if (!string.IsNullOrEmpty(resourceParams.UserName))
            {
                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.UserName == resourceParams.UserName);
            }

            //filter by type if type exists
            if (!string.IsNullOrEmpty(resourceParams.Type))
            {
                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.Type == resourceParams.Type);
            }

            //filter by name if name =||=
            if (!string.IsNullOrEmpty(resourceParams.Name))
            {
                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.Name == resourceParams.Name);
            }

            //searh if exists
            if (!string.IsNullOrEmpty(resourceParams.SearchQuery))
            {
                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.Name.Contains(resourceParams.SearchQuery)
                    || a.Type.Contains(resourceParams.SearchQuery));
            }

            return PagedList<PlaylistAlbum>.Create(collectionBeforePaging, resourceParams.PageNumber, resourceParams.PageSize);
        }

        public async Task<PlaylistAlbum> GetByIdAsync(int id)
        {
            var album = await _context.PlaylistAlbums.Include(t => t.Tracks).FirstOrDefaultAsync(a => a.PlaylistAlbumId == id);

            return album;
        }

        public bool GetByName(string name, string username)
        {

            if(_context.PlaylistAlbums.FirstOrDefault(a => a.Name == name && a.UserName == username) != null)
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

        public void Update(int id, PlaylistAlbum t)
        {
            var playlistAlbum = _context.PlaylistAlbums
                .Include(track => track.Tracks)
                .FirstOrDefault(a => a.PlaylistAlbumId == id);

            playlistAlbum.ImgUri = t.ImgUri;
            playlistAlbum.Name = t.Name;
            playlistAlbum.PlaylistAlbumId = t.PlaylistAlbumId;
            playlistAlbum.Tracks = t.Tracks;
            playlistAlbum.Type = t.Type;

            _context.PlaylistAlbums.Update(playlistAlbum);

        }
    }
}
