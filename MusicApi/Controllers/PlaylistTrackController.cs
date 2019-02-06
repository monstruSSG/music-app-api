using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using SpotifyApi.Domain.EntityModels;
using SpotifyApi.Domain.Logic.Links;
using SpotifyApi.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Controllers
{
    [Authorize(AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistTrackController : ControllerBase
    {
        private readonly IPlaylistTrackRepo _playlistTrackRepo;
        private readonly IMapper _mapper;
        private readonly ILinkService<PlaylistTrackDto, PlaylistTrackResourceParameters> _linkService;

        public PlaylistTrackController(IPlaylistTrackRepo playlistTrackRepo,
            IMapper mapper,
            ILinkService<PlaylistTrackDto, PlaylistTrackResourceParameters> linkService)
        {
            _playlistTrackRepo = playlistTrackRepo;
            _mapper = mapper;
            _linkService = linkService;
        }

        [HttpGet(Name = "GetPlaylistTracks")]
        public async Task<IActionResult> Get([FromQuery] PlaylistTrackResourceParameters resourceParameters)
        {
            var tracks = _playlistTrackRepo.GetAllPaginationAsync(resourceParameters);
            var mappedTracks = _mapper.Map<IEnumerable<PlaylistTrackDto>>(tracks);

            //construct links to previus+next page
            var previousPage = tracks.HasPrevious ?
               _linkService.CreateResourceUri(resourceParameters, ResourceType.PreviousPage) : null;

            var nextPage = tracks.HasNext ?
                _linkService.CreateResourceUri(resourceParameters, ResourceType.NextPage) : null;

            mappedTracks= mappedTracks.Select(track =>
            {
                track = _linkService.CreateLinks(track);
                return track;
            });

            var paginationMetadata = new
            {
                totalCount = tracks.TotalCount,
                pageSize = tracks.PageSize,
                currentPage = tracks.CurrentPage,
                totalPages = tracks.TotalPages,
                previousPageLink = previousPage,
                nextPageLink = nextPage
            };

            return Ok(new
            {
                Values = mappedTracks,
                Links = paginationMetadata
            });
        }

        [HttpPost(Name = "CreatePlaylistTrack")]
        public async Task<IActionResult> Post([FromBody] PlaylistTrackDto trackDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var track = _mapper.Map<PlaylistTrack>(trackDto);

            _playlistTrackRepo.Add(track);

            await _playlistTrackRepo.SaveChangesAsync();

            var mappedTrack = _mapper.Map<PlaylistTrackDto>(track);

            return Ok(_linkService.CreateLinks(mappedTrack));
        }

        [HttpDelete("{id}", Name = "DeletePlaylistTrack")]
        public async Task<IActionResult> Delete(int id)
        {
            //get the album
            var album = await _playlistTrackRepo.GetByIdAsync(id);

            if (album == null)
            {
                return BadRequest("Album with {id} not found");
            }

            //delete the album
            _playlistTrackRepo.Delete(album);

            //save changes async
            await _playlistTrackRepo.SaveChangesAsync();


            return StatusCode(204);
        }
    }
}
