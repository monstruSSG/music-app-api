using SpotifyApi.Domain.Dtos.ResourceParameters;
using SpotifyApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Services.IRepos
{
    public interface IUserRepo : IPagination<User, UserResourceParameters>
    {
    }
}
