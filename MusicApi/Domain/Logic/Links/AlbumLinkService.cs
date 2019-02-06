using Microsoft.AspNetCore.Mvc;
using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Logic.Links
{
    public class AlbumLinkService : ILinkService<AlbumDto, AlbumResourceParameters>
    {
        private readonly IUrlHelper _urlHelper;
        private readonly ILinkService<TrackDto, TrackResourceParameters> _trackLinkService;

        public AlbumLinkService(IUrlHelper urlHelper,
           ILinkService<TrackDto, TrackResourceParameters> trackLinkService)
        {
            _urlHelper = urlHelper;
            _trackLinkService = trackLinkService;
        }

        public AlbumDto CreateLinks(AlbumDto t)
        {
            t.Links.Add(new Link(_urlHelper.Link("GetAlbums",
               new { }),
               "get_all",
               "GET"));

            t.Links.Add(new Link(_urlHelper.Link("GetAlbumById",
              new { id = t.AlbumId }),
              "self",
              "GET"));

            t.Links.Add(new Link(_urlHelper.Link("DeleteAlbum",
            new { id = t.AlbumId }),
              "delete_album",
              "DELETE"));

            t.Links.Add(new Link(_urlHelper.Link("UpdateAlbum",
             new { id = t.AlbumId }),
            "update_self",
            "PUT"));

            t.Links.Add(new Link(_urlHelper.Link("AddTrackToAlbum",
               new { id = t.AlbumId }),
               "add_track_to_album",
               "PATCH"));

            t.Tracks = t.Tracks.Select(track => 
            {
                track = _trackLinkService.CreateLinks(track);

                return track;
            });

            return t;
        }

        public string CreateResourceUri(AlbumResourceParameters resourceParameters, ResourceType type)
        {
            switch (type)
            {
                case ResourceType.PreviousPage:
                    return _urlHelper.Link("GetAlbums",
                        new
                        {
                            orderBy = resourceParameters.OrderBy,
                            searchQuery = resourceParameters.SearchQuery,
                            name = resourceParameters.Name,
                            type = resourceParameters.Type,
                            pageNumber = resourceParameters.PageNumber - 1,
                            pageSize = resourceParameters.PageSize
                        });
                case ResourceType.NextPage:
                    return _urlHelper.Link("GetAlbums",
                        new
                        {
                            orderBy = resourceParameters.OrderBy,
                            searchQuery = resourceParameters.SearchQuery,
                            name = resourceParameters.Name,
                            type = resourceParameters.Type,
                            pageNumber = resourceParameters.PageNumber + 1,
                            pageSize = resourceParameters.PageSize
                        });
                default:
                    return _urlHelper.Link("GetAlbums",
                        new
                        {
                            orderBy = resourceParameters.OrderBy,
                            searchQuery = resourceParameters.SearchQuery,
                            name = resourceParameters.Name,
                            type = resourceParameters.Type,
                            pageNumber = resourceParameters.PageNumber,
                            pageSize = resourceParameters.PageSize
                        });
            }
        }
    }
}
