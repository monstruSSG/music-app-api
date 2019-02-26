using Microsoft.AspNetCore.Mvc;
using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using System;
using System.Linq;

namespace SpotifyApi.Domain.Logic.Links
{
    public class TrackLinkService : ILinkService<TrackDto, TrackResourceParameters>
    {
        private readonly IUrlHelper _urlHelper;
        private readonly ILinkService<ArtistDto, ArtistResourceParameters> _artistLinkService;

        public TrackLinkService(IUrlHelper urlHelper,
            ILinkService<ArtistDto, ArtistResourceParameters> artistLinkService)
        {
            _urlHelper = urlHelper;
            _artistLinkService = artistLinkService;
        }

        public TrackDto CreateLinks(TrackDto track)
        {

            track.Links.Add(new Link(_urlHelper.Link("GetTracks",
                new { }),
                "get_all",
                "GET"));

            track.Links.Add(new Link(_urlHelper.Link("DeleteTrack",
             new { id = track.TrackId }),
             "delete_track",
             "DELETE"));


            track.Links.Add(new Link(_urlHelper.Link("GetTrackById",
             new { id = track.TrackId }),
             "self",
             "GET"));


            track.Links.Add(new Link(_urlHelper.Link("UpdateTrack",
             new { id = track.TrackId }),
             "update_self",
             "PUT"));


            track.Links.Add(new Link(_urlHelper.Link("AddArtistToTrack",
             new { id = track.TrackId }),
             "add_artist_to_track",
             "PATCH"));

            track.Links.Add(new Link(_urlHelper.Link("CreateTrack",
             new { id = track.TrackId }),
             "create_track",
             "POST"));

            track.Artists = track.Artists.Select(artist =>
            {
                artist = _artistLinkService.CreateLinks(artist);

                return artist;
            });

            return track;
        }

        public TrackDto CreateLinksWhenDeleted(TrackDto t)
        {
            t.Links.Add(new Link(_urlHelper.Link("GetTracks",
               new { }),
               "get_all",
               "GET"));

            t.Links.Add(new Link(_urlHelper.Link("CreateTrack",
            new { id = t.TrackId }),
            "create_track",
            "POST"));

            t.Artists = t.Artists.Select(artist =>
            {
                artist = _artistLinkService.CreateLinksWhenDeleted(artist);

                return artist;
            });
            
            return t;
        }

        public string CreateResourceUri(TrackResourceParameters resourceParameters,
                ResourceType type)
        {
            switch (type)
            {
                case ResourceType.PreviousPage:
                    return _urlHelper.Link("GetTracks",
                        new
                        {
                            searchQuery = resourceParameters.SearchQuery,
                            name = resourceParameters.Name,
                            pageNumber = resourceParameters.PageNumber - 1,
                            pageSize = resourceParameters.PageSize
                        });
                case ResourceType.NextPage:
                    return _urlHelper.Link("GetTracks",
                        new
                        {
                            searchQuery = resourceParameters.SearchQuery,
                            name = resourceParameters.Name,
                            pageNumber = resourceParameters.PageNumber + 1,
                            pageSize = resourceParameters.PageSize
                        });
                default:
                    return _urlHelper.Link("GetTracks",
                        new
                        {
                            searchQuery = resourceParameters.SearchQuery,
                            name = resourceParameters.Name,
                            pageNumber = resourceParameters.PageNumber,
                            pageSize = resourceParameters.PageSize
                        });
            }

        }

    }
}
