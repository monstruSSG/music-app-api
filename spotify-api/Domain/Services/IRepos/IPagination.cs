using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Services.IRepos
{
    public interface IPagination<T, Resource>
        where T : class
        where Resource : BaseResourceParameters
    {
        PagedList<T> GetAllPagination(Resource resourceParameter);
    }
}
