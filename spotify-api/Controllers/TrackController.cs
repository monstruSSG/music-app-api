using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyApi.Domain.Models;
using SpotifyApi.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AutoMapper;
using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Logic.Links;
using System.Collections.Generic;
using System.Linq;
using SpotifyApi.Domain.Dtos.ResourceParameters;

namespace SpotifyApi.Controllers
{
    [Authorize(AuthenticationSchemes =
        JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class TrackController : ControllerBase
    {
        private readonly ITrackRepo _trackRepo;
        private readonly IMapper _mapper;
        private readonly ILinkService<TrackDto, TrackResourceParameters> _linkService;


        public TrackController(ITrackRepo trackRepo, 
            IMapper mapper,
            ILinkService<TrackDto, TrackResourceParameters> linkService)
        {
            _trackRepo = trackRepo;
            _mapper = mapper;

            _linkService = linkService;
        }

        /// <summary>
        /// Gets a paged list of all Tracks
        /// </summary>
        /// <remarks>
        /// Sample header:
        /// Authentication: Bearer {token}
        /// Sample request:
        ///
        ///     GET /api/track
        ///
        /// </remarks>
        /// <returns>A  paged list of Tracks</returns>
        /// <response code="200"></response>  
        [HttpGet(Name = "GetTracks")]
        public async Task<IActionResult> Get([FromQuery] TrackResourceParameters resourceParameters)
        {
            //Also must modify paginationAsync name since it is not async
            var tracks = _trackRepo.GetAllPagination(resourceParameters);

            var mappedTracks = _mapper.Map<IEnumerable<TrackDto>>(tracks);

            //constructing links to previous and next pages
            var previousPage = tracks.HasPrevious ?
                _linkService.CreateResourceUri(resourceParameters, ResourceType.PreviousPage) : null;

            var nextPage = tracks.HasNext ?
                _linkService.CreateResourceUri(resourceParameters, ResourceType.NextPage) : null;


            mappedTracks = mappedTracks.Select(track =>
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
        /// Gets a specific Track by {id}
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/track/{id}
        ///     {
        ///        "id": 1,
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Required</param>
        /// <returns>The track with the given id</returns>
        /// <response code="200">Returns the track</response>
        /// <response code="400">If the request has no id</response>   
        /// <response code="404">Track with given id not found</response> 
        [HttpGet("{id}", Name = "GetTrackById")]
        public async Task<IActionResult> Get(int id)
        {
            var track = await _trackRepo.GetByIdAsync(id);
        
            if (track == null)
            {
                return NotFound();
            }

            var mappedTrack = _mapper.Map<TrackDto>(track);

            return Ok(_linkService.CreateLinks(mappedTrack));
        }

        /// <summary>
        /// Creates a specific Track 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/album
        ///     {
        ///        "name": "Best Album",
        ///        "previewUrl": "http://listen.com/liste_me",
        ///        "href": "http://album.com/track",
        ///        "artists": []
        ///     }
        ///
        /// </remarks>
        /// <returns>The Track </returns>
        /// <response code="200">Returns the created track</response>
        /// <response code="400">Invalid model</response> 
        [HttpPost(Name = "CreateTrack")]
        public async Task<IActionResult> Post([FromBody] TrackToCreateDto trackDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var track = _mapper.Map<Track>(trackDto);

            _trackRepo.Add(track);

            await _trackRepo.SaveChangesAsync();

            var mappedTrack = _mapper.Map<TrackDto>(track);

            return Ok(_linkService.CreateLinks(mappedTrack));
        }

         /// <summary>
         /// Updates a specific Track by {id}
         /// </summary>
         /// <remarks>
         /// Sample request:
         ///
         ///     PUT /api/album/{id}
         ///     {
         ///        "name": "Best Album",
         ///        "previewUrl": "http://listen.com/liste_me",
         ///        "href": "http://album.com/track",
         ///        "artists": []
         ///     }
         ///
         /// </remarks>
         /// <param name="id">Required</param>
         /// <returns>The Track with the given id</returns>
         /// <response code="200">Returns the Track</response>
         /// <response code="400">If the request has no id or invalid album model</response>   
         /// <response code="404">Track with given id not found</response>  
         [HttpPut("{id}", Name = "UpdateTrack")]
        public async Task<IActionResult> Update(int id, [FromBody] TrackToCreateDto trackDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var mappedTrack = _mapper.Map<Track>(trackDto);

            _trackRepo.Update(id, mappedTrack);

            await _trackRepo.SaveChangesAsync();

            var newTrack = await _trackRepo.GetByIdAsync(id);

            var updatedTrack = _mapper.Map<TrackDto>(newTrack);

            return Ok(_linkService.CreateLinks(updatedTrack));
        }

        /// <summary>
        /// Deletes a specific track
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/track/{id}
        ///     {
        ///        "id": 1,
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Required</param>
        /// <returns>The track with the given id</returns>
        /// <response code="200">Track</response>
        /// <response code="400">If the request has no id</response>   
        /// <response code="404">trac with given id not found</response>  
        [HttpDelete("{id}", Name = "DeleteTrack")]
        public async Task<IActionResult> Delete(int id)
        {
            var track = await _trackRepo.GetByIdAsync(id);

            if(track == null)
            {
                return NotFound();
            }

            _trackRepo.Delete(track);

            await _trackRepo.SaveChangesAsync();

            var mappedTrack = _mapper.Map<TrackDto>(track);
            
            return Ok(_linkService.CreateLinksWhenDeleted(mappedTrack));
        }

        /// <summary>
        /// Adds a track to a specifictrack
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH /{id}/track
        ///     {  
        ///        "name": "Best Artist"
        ///         
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Required</param>
        /// <returns>The track with the given id</returns>
        /// <response code="200">Returns the track with new track</response>
        /// <response code="400">If the request has no id or invalid track model</response>   
        /// <response code="404">track with given id not found</response>  
        [HttpPatch("{id}/artist", Name = "AddArtistToTrack")]
        public async Task<IActionResult> AddArtistToTrack(int id, [FromBody] ArtistToCreateDto artistDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var artist = _mapper.Map<Artist>(artistDto);

            var track = await _trackRepo.AddArtistToTrack(id, artist);
            await _trackRepo.SaveChangesAsync();

            var mappedTrack = _mapper.Map<TrackDto>(track);
            
            return Ok(_linkService.CreateLinks(mappedTrack));
        }

        [HttpGet("play/{id}", Name = "PlayTrack")]
        public async Task<IActionResult> PlayTrack(int id)
        {
            return null;
        }

    }

}
