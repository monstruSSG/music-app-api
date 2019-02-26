using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Services.IRepos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Services
{
    public interface IRepo<T, Resource> : IPagination<T, Resource>
        where T : class
        where Resource: BaseResourceParameters
    {
        void Add(T t);
        void Delete(T t);
        void Update(int id, T t);
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<bool> SaveChangesAsync();
        
    }
}
