using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using SpotifyApi.Domain.EntityModels;
using SpotifyApi.Domain.Logic;

namespace SpotifyApi.Domain.Services
{
    public class RequestRepo : IRequestRepo
    {
        private readonly DataContext _context;

        public RequestRepo(DataContext context)
        {
            _context = context;
        }

        public void Add(Request t)
        {
            _context.Requests.Add(t);
        }

        public void Delete(Request t)
        {
            _context.Remove(t);
        }

        public async Task<List<Request>> GetAllAsync()
        {
            return await _context.Requests
                .Include(request => request.UserAgent)
                .ToListAsync();
        }

        public PagedList<Request> GetAllPagination(RequestResourceParameters resourceParams)
        {
            var collectionBeforPaging = _context.Requests
                .Include(r => r.UserAgent)
                .OrderBy(r => r.DateTime)
                .AsQueryable();

            //filter by Method if type exists
            if (!string.IsNullOrEmpty(resourceParams.Method))
            {
                collectionBeforPaging = collectionBeforPaging
                    .Where(a => a.Method == resourceParams.Method);
            }

            //filter by Destination if type exists
            if (!string.IsNullOrEmpty(resourceParams.Destination))
            {
                collectionBeforPaging = collectionBeforPaging
                    .Where(a => a.Destination == resourceParams.Destination);
            }

            //filter by Source if type exists
            if (!string.IsNullOrEmpty(resourceParams.Source))
            {
                collectionBeforPaging = collectionBeforPaging
                    .Where(a => a.Source == resourceParams.Source);
            }

            //searh if exists
            if (!string.IsNullOrEmpty(resourceParams.SearchQuery))
            {
                collectionBeforPaging = collectionBeforPaging
                    .Where(a => a.Method.Contains(resourceParams.SearchQuery)
                    || a.Destination.Contains(resourceParams.SearchQuery)
                    || a.Source.Contains(resourceParams.SearchQuery));
            }

            return PagedList<Request>.Create(collectionBeforPaging, resourceParams.PageNumber, resourceParams.PageSize);
        }

        public async Task<Request> GetByIdAsync(int id)
        {
            return await _context.Requests
                .Include(request => request.UserAgent)
                .FirstOrDefaultAsync(request => request.RequestId == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            //returntrue if 1 or more entities were changed
            return (await _context.SaveChangesAsync() > 0);
        }

        public void Update(int id, Request t)
        {
            var request = _context.Requests
                .Include(r => r.UserAgent)
                .FirstOrDefault(r => r.RequestId == id);
            
            _context.Requests.Update(request);
        }
    }
}
