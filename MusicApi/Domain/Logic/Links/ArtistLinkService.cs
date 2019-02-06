using Microsoft.AspNetCore.Mvc;
using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Logic.Links
{
    public class ArtistLinkService : ILinkService<ArtistDto, ArtistResourceParameters>
    {
        private readonly IUrlHelper _urlHelper;

        public ArtistLinkService(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public ArtistDto CreateLinks(ArtistDto t)
        {
            t.Links.Add(new Link(_urlHelper.Link("GetArtists",
                 new { }),
                 "get_all",
                 "GET"));

            t.Links.Add(new Link(_urlHelper.Link("DeleteArtist",
             new { id = t.ArtistId }),
             "delete_artist",
             "DELETE"));


            t.Links.Add(new Link(_urlHelper.Link("GetArtistById",
             new { id = t.ArtistId }),
             "self",
             "GET"));


            t.Links.Add(new Link(_urlHelper.Link("UpdateArtist",
             new { id = t.ArtistId }),
             "update_self",
             "PUT"));

            return t;
        }

        public string CreateResourceUri(ArtistResourceParameters resourceParameters, ResourceType type)
        {

            switch (type)
            {
                case ResourceType.PreviousPage:
                    return _urlHelper.Link("GetArtists",
                        new
                        {
                            orderBy = resourceParameters.OrderBy,
                            searchQuery = resourceParameters.SearchQuery,
                            name = resourceParameters.Name,
                            pageNumber = resourceParameters.PageNumber - 1,
                            pageSize = resourceParameters.PageSize
                        });
                case ResourceType.NextPage:
                    return _urlHelper.Link("GetArtists",
                        new
                        {
                            orderBy = resourceParameters.OrderBy,
                            searchQuery = resourceParameters.SearchQuery,
                            name = resourceParameters.Name,
                            pageNumber = resourceParameters.PageNumber + 1,
                            pageSize = resourceParameters.PageSize
                        });
                default:
                    return _urlHelper.Link("GetArtists",
                        new
                        {
                            orderBy = resourceParameters.OrderBy,
                            searchQuery = resourceParameters.SearchQuery,
                            name = resourceParameters.Name,
                            pageNumber = resourceParameters.PageNumber,
                            pageSize = resourceParameters.PageSize
                        });
            }
        }
    }
}
