using Microsoft.AspNetCore.Mvc;
using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Logic.Links
{
    public class PlaylistTrackLinkService : ILinkService<PlaylistTrackDto, PlaylistTrackResourceParameters>
    {
        private readonly IUrlHelper _urlHelper;

        public PlaylistTrackLinkService(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public PlaylistTrackDto CreateLinks(PlaylistTrackDto t)
        {
            t.Links.Add(new Link(_urlHelper.Link("GetPlaylistTracks",
            new { }),
            "get_all",
            "GET"));

            t.Links.Add(new Link(_urlHelper.Link("CreatePlaylistTrack",
              new { }),
              "post_playlistTrack",
              "POST"));

            t.Links.Add(new Link(_urlHelper.Link("DeletePlaylistTrack",
             new { id = t.PlaylistTrackId }),
             "delete_playlistTrack",
             "DELETE"));


            return t;
        }

        public string CreateResourceUri(PlaylistTrackResourceParameters resourceParameters, ResourceType type)
        {
            switch (type)
            {
                case ResourceType.PreviousPage:
                    return _urlHelper.Link("GetPlaylistTracks",
                        new
                        {
                            orderBy = resourceParameters.OrderBy,
                            searchQuery = resourceParameters.SearchQuery,
                            name = resourceParameters.Name,
                            pageNumber = resourceParameters.PageNumber - 1,
                            pageSize = resourceParameters.PageSize
                        });
                case ResourceType.NextPage:
                    return _urlHelper.Link("GetPlaylistTracks",
                        new
                        {
                            orderBy = resourceParameters.OrderBy,
                            searchQuery = resourceParameters.SearchQuery,
                            name = resourceParameters.Name,
                            pageNumber = resourceParameters.PageNumber + 1,
                            pageSize = resourceParameters.PageSize
                        });
                default:
                    return _urlHelper.Link("GetPlaylistTracks",
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
