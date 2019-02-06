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
    public class PlaylistTrackRepo : IPlaylistTrackRepo
    {

        private readonly DataContext _context;

        public PlaylistTrackRepo(DataContext context) 
        {
            _context = context;
        }

        public void Add(PlaylistTrack t)
        {
            _context.PlaylistTracks.Add(t);
        }

        public void Delete(PlaylistTrack t)
        {
            _context.PlaylistTracks.Remove(t);
        }

        public async Task<List<PlaylistTrack>> GetAllAsync()
        {
            return await _context.PlaylistTracks.ToListAsync();
        }

        public PagedList<PlaylistTrack> GetAllPaginationAsync(PlaylistTrackResourceParameters resourceParams)
        {
            var collectionBeforPaging = _context.PlaylistTracks
                .AsQueryable();

            //filter by name if name =||=
            if (!string.IsNullOrEmpty(resourceParams.Name))
            {
                collectionBeforPaging = collectionBeforPaging
                    .Where(a => a.Name == resourceParams.Name);
            }

            //searh if exists
            if (!string.IsNullOrEmpty(resourceParams.SearchQuery))
            {
                collectionBeforPaging = collectionBeforPaging
                    .Where(a => a.Name.Contains(resourceParams.SearchQuery));
            }


            return PagedList<PlaylistTrack>.Create(collectionBeforPaging, resourceParams.PageNumber, resourceParams.PageSize);
        }

        public async Task<PlaylistTrack> GetByIdAsync(int id)
        {
            var track = await _context.PlaylistTracks.FirstOrDefaultAsync(tr => tr.PlaylistTrackId == id);

            return track;
        }

        public async Task<bool> SaveChangesAsync()
        {
            //returntrue if 1 or more entities were changed
            return (await _context.SaveChangesAsync() > 0);
        }

        public void Update(int id, PlaylistTrack t)
        {
            var playlistTrack = _context.PlaylistTracks.FirstOrDefault(a => a.PlaylistTrackId == id);

            playlistTrack.Name = t.Name;
            playlistTrack.PlaylistTrackId = t.PlaylistTrackId;
            playlistTrack.Href = t.Href;
            playlistTrack.PreviewUrl = t.PreviewUrl;
            
            _context.PlaylistTracks.Update(playlistTrack);
        }
    }
}
