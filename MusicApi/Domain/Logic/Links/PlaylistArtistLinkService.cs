using Microsoft.AspNetCore.Mvc;
using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Logic.Links
{
    public class PlaylistArtistLinkService : ILinkService<PlaylistArtistDto, PlaylistArtistResourceParameters>
    {
        private readonly IUrlHelper _urlHelper;

        public PlaylistArtistLinkService(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public PlaylistArtistDto CreateLinks(PlaylistArtistDto t)
        {
            t.Links.Add(new Link(_urlHelper.Link("GetPlaylistArtists",
              new { }),
              "get_all",
              "GET"));

            t.Links.Add(new Link(_urlHelper.Link("CreatePlaylistArtist",
              new { }),
              "post_playlistArtist",
              "POST"));

            t.Links.Add(new Link(_urlHelper.Link("DeletePlaylistArtist",
              new { id = t.PlaylistArtistId }),
              "delete_playlistArtist",
              "DELETE"));


            return t;
        }

        public string CreateResourceUri(PlaylistArtistResourceParameters resourceParameters, ResourceType type)
        {
            switch (type)
            {
                case ResourceType.PreviousPage:
                    return _urlHelper.Link("GetPlaylistArtists",
                        new
                        {
                            orderBy = resourceParameters.OrderBy,
                            searchQuery = resourceParameters.SearchQuery,
                            name = resourceParameters.Name,
                            pageNumber = resourceParameters.PageNumber - 1,
                            pageSize = resourceParameters.PageSize
                        });
                case ResourceType.NextPage:
                    return _urlHelper.Link("GetPlaylistArtists",
                        new
                        {
                            orderBy = resourceParameters.OrderBy,
                            searchQuery = resourceParameters.SearchQuery,
                            name = resourceParameters.Name,
                            pageNumber = resourceParameters.PageNumber + 1,
                            pageSize = resourceParameters.PageSize
                        });
                default:
                    return _urlHelper.Link("GetPlaylistArtists",
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
