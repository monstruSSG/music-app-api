using Microsoft.EntityFrameworkCore;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using SpotifyApi.Domain.Logic;
using SpotifyApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Services
{
    public class ArtistRepo : IArtistRepo 
    {
        private readonly DataContext _context;

        public ArtistRepo(DataContext context) 
        {
            _context = context;
        }


        public void Add(Artist t)
        {
            _context.Artists.Add(t);
        }

        public void Delete(Artist t)
        {
            _context.Artists.Remove(t);
        }

        public async Task<List<Artist>> GetAllAsync()
        {
            return await _context.Artists.ToListAsync();
        }

        public PagedList<Artist> GetAllPaginationAsync(ArtistResourceParameters resourceParams)
        {
            var collectionBeforPaging = _context.Artists.AsQueryable();

            //if name filter exists
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

            return PagedList<Artist>.Create(collectionBeforPaging, resourceParams.PageNumber, resourceParams.PageSize);
        }

        public async Task<Artist> GetByIdAsync(int id)
        {
            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.ArtistId == id);

            return artist;
        }

        public async Task<bool> SaveChangesAsync()
        {
            //returntrue if 1 or more entities were changed
            return (await _context.SaveChangesAsync() > 0);
        }

        public void Update(int id, Artist newArtist)
        {
            var artist = _context.Artists.FirstOrDefault(a => a.ArtistId == id);
            
            artist.ImgUri = newArtist.ImgUri;
            artist.Name = newArtist.Name;
            artist.Uri = newArtist.Uri;

            _context.Artists.Update(artist);

        }
    }
}
