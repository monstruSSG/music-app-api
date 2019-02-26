using Microsoft.AspNetCore.Mvc;
using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Logic.Links
{
    public class PlaylistAlbumLinkService : ILinkService<PlaylistAlbumDto, PlaylistAlbumResourceParameters>
    {
        private readonly IUrlHelper _urlHelper;
        private readonly ILinkService<PlaylistTrackDto, PlaylistTrackResourceParameters> _trackLinkService;

        public PlaylistAlbumLinkService(IUrlHelper urlHelper,
            ILinkService<PlaylistTrackDto, PlaylistTrackResourceParameters> trackLinkService)
        {
            _urlHelper = urlHelper;
            _trackLinkService = trackLinkService;
        }

        public PlaylistAlbumDto CreateLinks(PlaylistAlbumDto t)
        {

            t.Links.Add(new Link(_urlHelper.Link("GetPlaylistAlbums",
               new { }),
               "get_all",
               "GET"));

            t.Links.Add(new Link(_urlHelper.Link("CreatePlaylistAlbum",
              new { }),
              "post_playlistAlbum",
              "POST"));

            t.Links.Add(new Link(_urlHelper.Link("DeletePlaylistAlbum",
              new { id = t.PlaylistAlbumId }),
              "delete_playlistAlbum",
              "DELETE"));

            t.Tracks = t.Tracks.Select(track => 
            {
                track = _trackLinkService.CreateLinks(track);

                return track;
            });

            return t;
        }

        public PlaylistAlbumDto CreateLinksWhenDeleted(PlaylistAlbumDto t)
        {
            t.Links.Add(new Link(_urlHelper.Link("GetPlaylistAlbums",
               new { }),
               "get_all",
               "GET"));

            t.Links.Add(new Link(_urlHelper.Link("CreatePlaylistAlbum",
              new { }),
              "post_playlistAlbum",
              "POST"));


            t.Tracks = t.Tracks.Select(track =>
            {
                track = _trackLinkService.CreateLinksWhenDeleted(track);

                return track;
            });

            return t;

        }

        public string CreateResourceUri(PlaylistAlbumResourceParameters resourceParameters, ResourceType type)
        {
            switch (type)
            {
                case ResourceType.PreviousPage:
                    return _urlHelper.Link("GetPlaylistAlbums",
                        new
                        {
                            userName = resourceParameters.UserName,
                            searchQuery = resourceParameters.SearchQuery,
                            name = resourceParameters.Name,
                            type = resourceParameters.Type,
                            pageNumber = resourceParameters.PageNumber - 1,
                            pageSize = resourceParameters.PageSize
                        });
                case ResourceType.NextPage:
                    return _urlHelper.Link("GetPlaylistAlbums",
                        new
                        {
                            userName = resourceParameters.UserName,
                            searchQuery = resourceParameters.SearchQuery,
                            name = resourceParameters.Name,
                            type = resourceParameters.Type,
                            pageNumber = resourceParameters.PageNumber + 1,
                            pageSize = resourceParameters.PageSize
                        });
                default:
                    return _urlHelper.Link("GetPlaylistAlbums",
                        new
                        {
                            userName = resourceParameters.UserName,
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
