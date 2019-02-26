using Microsoft.EntityFrameworkCore;
using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using SpotifyApi.Domain.Logic;
using SpotifyApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Services
{
    public class TrackRepo : ITrackRepo
    {
        private readonly DataContext _context;

        public TrackRepo(DataContext context) 
        {
            _context = context;
        }

        public void Add(Track t)
        {
            _context.Add(t);
        }

        public async Task<Track> AddArtistToTrack(int id, Artist artist)
        {
            var t = await GetByIdAsync(id);

            if (t.Artists == null)
            {
                t.Artists = new List<Artist>();
            }

            t.Artists.Add(artist);

            return t;
        }

        public void Delete(Track t)
        {
            _context.Remove(t);
        }

        public async Task<List<Track>> GetAllAsync()
        {
            return await _context.Tracks
                .Include(p => p.Artists)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public PagedList<Track> GetAllPagination(TrackResourceParameters resourceParams)
        {
            var collectionBeforPaging = _context.Tracks
                .Include(a => a.Artists)
                .AsQueryable();

            //filter by name if exists
            if(!string.IsNullOrEmpty(resourceParams.Name))
            {
                collectionBeforPaging = collectionBeforPaging
                    .Where(t => t.Name == resourceParams.Name);
            }

            //searh if exists
            if (!string.IsNullOrEmpty(resourceParams.SearchQuery))
            {
                collectionBeforPaging = collectionBeforPaging
                    .Where(a => a.Name.Contains(resourceParams.SearchQuery));
            }

            return PagedList<Track>.Create(collectionBeforPaging, resourceParams.PageNumber, resourceParams.PageSize);
        }

        public async Task<Track> GetByIdAsync(int id)
        {
            return await _context.Tracks
                .Include(a => a.Artists)
                .FirstOrDefaultAsync(t => t.TrackId == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            //returntrue if 1 or more entities were changed
            return (await _context.SaveChangesAsync() > 0);
        }

        public void Update(int id, Track newTrack)
        {
            var track = _context.Tracks.FirstOrDefault(t => t.TrackId == id);

            track.Artists = newTrack.Artists;
            track.Name = newTrack.Name;
            track.PreviewUrl = newTrack.PreviewUrl;
            track.Href = newTrack.Href;

            _context.Tracks.Update(track);
        }
    }
}
