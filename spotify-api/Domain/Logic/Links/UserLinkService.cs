using Microsoft.AspNetCore.Mvc;
using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Logic.Links
{
    public class UserLinkService : ILinkService<UserDto, UserResourceParameters>
    {
        private readonly IUrlHelper _urlHelper;

        public UserLinkService(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public UserDto CreateLinks(UserDto t)
        {

            t.Links.Add(new Link(_urlHelper.Link("CreateUser",
               new { }),
               "create_new_user",
               "POST"));

            t.Links.Add(new Link(_urlHelper.Link("Login",
               new { }),
               "login",
               "POST"));

            t.Links.Add(new Link(_urlHelper.Link("CreateAdmin",
               new { }),
               "crete_new_admin_user",
               "POST"));

            t.Links.Add(new Link(_urlHelper.Link("GetUsers",
               new { }),
               "gets_all_users",
               "GET"));

            return t;
        }

        public UserDto CreateLinksWhenDeleted(UserDto t)
        {
            t.Links.Add(new Link(_urlHelper.Link("CreateUser",
             new { }),
             "create_new_user",
             "POST"));

            t.Links.Add(new Link(_urlHelper.Link("CreateAdmin",
             new { }),
             "crete_new_admin_user",
             "POST"));

            t.Links.Add(new Link(_urlHelper.Link("GetUsers",
              new { }),
              "gets_all_users",
              "GET"));


            return t;
        }

        public string CreateResourceUri(UserResourceParameters resourceParameters, ResourceType type)
        {
            switch (type)
            {
                case ResourceType.PreviousPage:
                    return _urlHelper.Link("GetUsers",
                        new
                        {
                            searchQuery = resourceParameters.SearchQuery,
                            pageNumber = resourceParameters.PageNumber - 1,
                            pageSize = resourceParameters.PageSize
                        });
                case ResourceType.NextPage:
                    return _urlHelper.Link("GetUsers",
                        new
                        {
                            searchQuery = resourceParameters.SearchQuery,
                            pageNumber = resourceParameters.PageNumber + 1,
                            pageSize = resourceParameters.PageSize
                        });
                default:
                    return _urlHelper.Link("GetUsers",
                        new
                        {
                            searchQuery = resourceParameters.SearchQuery,
                            pageNumber = resourceParameters.PageNumber,
                            pageSize = resourceParameters.PageSize
                        });
            }
        }
    }
}
