using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using SpotifyApi.Domain.EntityModels;
using SpotifyApi.Domain.Logic.Links;
using SpotifyApi.Domain.Models;
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
        private readonly UserManager<User> _userManager;

        public PlaylistTrackController(IPlaylistTrackRepo playlistTrackRepo,
            IMapper mapper,
            UserManager<User> userManager,
            ILinkService<PlaylistTrackDto, PlaylistTrackResourceParameters> linkService)
        {
            _playlistTrackRepo = playlistTrackRepo;
            _mapper = mapper;
            _linkService = linkService;
            _userManager = userManager;
        }

        /// <summary>
        /// Gets a paged list of all Tracks
        /// </summary>
        /// <remarks>
        /// Sample header:
        /// Authentication: Bearer {token}
        /// Sample request:
        ///
        ///     GET /api/playlisttrack
        ///
        /// </remarks>
        /// <returns>A  paged list of Tracks</returns>
        /// <response code="200"></response>  
        [HttpGet(Name = "GetPlaylistTracks")]
        public async Task<IActionResult> Get([FromQuery] PlaylistTrackResourceParameters resourceParameters)
        {
            var tracks = _playlistTrackRepo.GetAllPagination(resourceParameters);
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

        /// <summary>
        /// Creates a specific Track 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/playlisttrack
        ///     {
        ///        "userName": "celMaiTareUSer",
        ///        "name": "Best Album",
        ///        "previewUrl": "http://listen.com/liste_me",
        ///        "href": "http://album.com/track",
        ///     }
        ///
        /// </remarks>
        /// <returns>The Track </returns>
        /// <response code="200">Returns the created track</response>
        /// <response code="400">Invalid model</response> 
        [HttpPost(Name = "CreatePlaylistTrack")]
        public async Task<IActionResult> Post([FromBody] PlaylistTrackToCreateDto trackDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(trackDto.UserName);

            if (user == null)
            {
                return NotFound();
            }

            var track = _mapper.Map<PlaylistTrack>(trackDto);

            if (_playlistTrackRepo.GetByName(track.Name, user.UserName) == true)
            {
                return StatusCode(409);
            }

            _playlistTrackRepo.Add(track);

            await _playlistTrackRepo.SaveChangesAsync();

            var mappedTrack = _mapper.Map<PlaylistTrackDto>(track);

            return Ok(_linkService.CreateLinks(mappedTrack));
        }

        /// <summary>
        /// Deletes a specific track
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/playlisttrack/{id}
        ///     {
        ///        "id": 1,
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Required</param>
        /// <returns>The track with the given id</returns>
        /// <response code="200">PlaylistTrack</response>
        /// <response code="400">If the request has no id</response>   
        /// <response code="404">Track with given id not found</response>
        [HttpDelete("{id}", Name = "DeletePlaylistTrack")]
        public async Task<IActionResult> Delete(int id)
        {
            //get the album
            var track = await _playlistTrackRepo.GetByIdAsync(id);

            if (track == null)
            {
                return BadRequest("Album with {id} not found");
            }

            //delete the album
            _playlistTrackRepo.Delete(track);

            //save changes async
            await _playlistTrackRepo.SaveChangesAsync();

            var mappedTrack = _mapper.Map<PlaylistTrackDto>(track);

            return Ok(_linkService.CreateLinksWhenDeleted(mappedTrack));
        }
    }
}
