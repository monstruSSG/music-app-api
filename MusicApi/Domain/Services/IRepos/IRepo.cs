using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Services
{
    public interface IRepo<T, Resource> 
        where T : class
        where Resource: BaseResourceParameters
    {
        void Add(T t);
        void Delete(T t);
        void Update(int id, T t);
        Task<List<T>> GetAllAsync();
        PagedList<T> GetAllPaginationAsync(Resource resourceParameter);
        Task<T> GetByIdAsync(int id);
        Task<bool> SaveChangesAsync();
        
    }
}
