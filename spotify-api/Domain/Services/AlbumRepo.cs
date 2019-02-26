using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpotifyApi.Domain.Models;
using SpotifyApi.Domain.Logic;
using SpotifyApi.Domain.Dtos.ResourceParameters;

namespace SpotifyApi.Domain.Services
{
    public class AlbumRepo : IAlbmRepo
    {
        private readonly DataContext _context;

        public AlbumRepo(DataContext context)
        {
            _context = context;
        }

        public void Add(Album t)
        {
            _context.Albums.Add(t);
        }

        public async Task<Album> AddTrackToAlbum(int id, Track track)
        {
            //get album to add track
            var album = await GetByIdAsync(id);

            //now add track to album.tracks
            if (album.Tracks == null)
            {
                album.Tracks = new List<Track>();
            }

            //Add track to album
            album.Tracks.Add(track);

            return album;

        }

        public void Delete(Album t)
        {
            _context.Albums.Remove(t);

        }

        public async Task<Album> GetAlbumByNameAsync(string albumName)
        {
            var album = await _context.Albums.FirstOrDefaultAsync(a => a.Name == albumName);

            return album;
        }

        public async Task<List<Album>> GetAllAsync()
        {
            return await _context.Albums
                .Include(t => t.Tracks)
                .ToListAsync();
        }

        public PagedList<Album> GetAllPagination(AlbumResourceParameters resourceParams)
        {

            var collectionBeforPaging = _context.Albums
                .Include(t => t.Tracks)
                .OrderBy(t => t.Name)
                .AsQueryable();

             //filter by type if type exists
            if(!string.IsNullOrEmpty(resourceParams.Type))
            {
                collectionBeforPaging = collectionBeforPaging
                    .Where(a => a.Type == resourceParams.Type);
            }

            //filter by name if name =||=
            if (!string.IsNullOrEmpty(resourceParams.Name))
            {
                collectionBeforPaging = collectionBeforPaging
                    .Where(a => a.Name == resourceParams.Name);
            }


            //searh if exists
            if(!string.IsNullOrEmpty(resourceParams.SearchQuery))
            {
                collectionBeforPaging = collectionBeforPaging
                    .Where(a => a.Name.Contains(resourceParams.SearchQuery)
                    || a.Type.Contains(resourceParams.SearchQuery));
            }

            return PagedList<Album>.Create(collectionBeforPaging, resourceParams.PageNumber, resourceParams.PageSize);

        }

        public async Task<Album> GetByIdAsync(int id)
        {
            var album = await _context.Albums.Include(t => t.Tracks).FirstOrDefaultAsync(a => a.AlbumId == id);

            return album;
        }

        public async Task<bool> SaveChangesAsync()
        {
            //returntrue if 1 or more entities were changed
            return (await _context.SaveChangesAsync() > 0);
        }

        public void Update(int id, Album newAlbum)
        {
            var album = _context.Albums.Include(t => t.Tracks).FirstOrDefault(a => a.AlbumId == id);

            album.ImgUri = newAlbum.ImgUri;
            album.Name = newAlbum.Name;
            album.Type = newAlbum.Type;
            album.Tracks = newAlbum.Tracks;

            _context.Albums.Update(album);

        }
    }
}
