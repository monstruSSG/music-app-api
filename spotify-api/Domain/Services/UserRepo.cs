using Microsoft.AspNetCore.Identity;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using SpotifyApi.Domain.Logic;
using SpotifyApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Services.IRepos
{
    public class UserRepo : IUserRepo
    {
        private readonly UserManager<User> _userManager;

        public UserRepo(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public PagedList<User> GetAllPagination(UserResourceParameters resourceParams)
        {
            var collectionBeforPaging = _userManager.Users
                .OrderBy(u => u.UserName)
                .AsQueryable();

            //searh if exists
            if (!string.IsNullOrEmpty(resourceParams.SearchQuery))
            {
                collectionBeforPaging = collectionBeforPaging
                    .Where(a => a.NormalizedUserName.Contains(resourceParams.SearchQuery.ToUpper())
                    || a.NormalizedEmail.Contains(resourceParams.SearchQuery.ToUpper()));
            }

            return PagedList<User>.Create(collectionBeforPaging, resourceParams.PageNumber, resourceParams.PageSize);

        }
    }
}
