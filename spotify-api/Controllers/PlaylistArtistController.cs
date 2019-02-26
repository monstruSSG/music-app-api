using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using SpotifyApi.Domain.EntityModels;
using SpotifyApi.Domain.Logic.Links;
using SpotifyApi.Domain.Models;
using SpotifyApi.Domain.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Controllers
{
    [Authorize(AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistArtistController : ControllerBase
    {

        private readonly IPlaylistArtist _playlistArtistRepo;
        private readonly IMapper _mapper;
        private readonly ILinkService<PlaylistArtistDto, PlaylistArtistResourceParameters> _linkService;
        private readonly UserManager<User> _userManager;

        public PlaylistArtistController(IPlaylistArtist playlistArtistRepo, 
            IMapper mapper,
            UserManager<User> userManager,
            ILinkService<PlaylistArtistDto, PlaylistArtistResourceParameters> linkService)
        {
            _playlistArtistRepo = playlistArtistRepo;
            _mapper = mapper;
            _linkService = linkService;
            _userManager = userManager;
        }

        /// <summary>
        /// Gets a paged list of all Artists
        /// </summary>
        /// <remarks>
        /// Sample header:
        /// Authentication: Bearer {token}
        /// Sample request:
        ///
        ///     GET /api/playlistartist
        ///
        /// </remarks>
        /// <returns>A paged list of Artists</returns>
        /// <response code="200"></response> 
        [HttpGet(Name = "GetPlaylistArtists")]
        public async Task<IActionResult> Get([FromQuery] PlaylistArtistResourceParameters resourceParameters)
        {
            var artists = _playlistArtistRepo.GetAllPagination(resourceParameters);
            var mappedArtists = _mapper.Map<IEnumerable<PlaylistArtistDto>>(artists);

            //construct links to previus+next page
            var previousPage = artists.HasPrevious ?
               _linkService.CreateResourceUri(resourceParameters, ResourceType.PreviousPage) : null;

            var nextPage = artists.HasNext ?
                _linkService.CreateResourceUri(resourceParameters, ResourceType.NextPage) : null;

            mappedArtists = mappedArtists.Select(artist =>
            {
                artist = _linkService.CreateLinks(artist);
                return artist;
            });

            var paginationMetadata = new
            {
                totalCount = artists.TotalCount,
                pageSize = artists.PageSize,
                currentPage = artists.CurrentPage,
                totalPages = artists.TotalPages,
                previousPageLink = previousPage,
                nextPageLink = nextPage
            };

            return Ok(new
            {
                Values = mappedArtists,
                Links = paginationMetadata
            });
        }

        /// <summary>
        /// Creates a specific Artist 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/playlistartist
        ///     {
        ///        "userName": "celMaiTareUSer",
        ///        "name": "Best Album",
        ///        "imgUri": "http://photos.com/1",
        ///        "uri": "http://album.com/track",
        ///        "tracks": []
        ///     }
        ///
        /// </remarks>
        /// <returns>The Artist </returns>
        /// <response code="200">Returns the created Artist</response>
        /// <response code="400">Invalid model</response> 
        [HttpPost(Name = "CreatePlaylistArtist")]
        public async Task<IActionResult> Post([FromBody] PlaylistArtistToCreateDto artistDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(artistDto.UserName);

            if (user == null)
            {
                return NotFound();
            }


            var artist = _mapper.Map<PlaylistArtist>(artistDto);

            if (_playlistArtistRepo.GetByName(artist.Name, user.UserName) == true)
            {
                return StatusCode(409);
            }

            _playlistArtistRepo.Add(artist);

            await _playlistArtistRepo.SaveChangesAsync();

            var mappedArtist = _mapper.Map<PlaylistArtistDto>(artist);

            return Ok(_linkService.CreateLinks(mappedArtist));
        }

        /// <summary>
        /// Deletes a specific artist
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/playlistartist/{id}
        ///     {
        ///        "id": 1,
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Required</param>
        /// <returns>The artist with the given id</returns>
        /// <response code="200">PlaylistArtist</response>
        /// <response code="400">If the request has no id</response>   
        /// <response code="404">Artist with given id not found</response>
        [HttpDelete("{id}", Name = "DeletePlaylistArtist")]
        public async Task<IActionResult> Delete(int id)
        {
            //get the album
            var artist = await _playlistArtistRepo.GetByIdAsync(id);

            if (artist == null)
            {
                return BadRequest("Artist with {id} not found");
            }

            //delete the album
            _playlistArtistRepo.Delete(artist);

            //save changes async
            await _playlistArtistRepo.SaveChangesAsync();

            var mappedArtist = _mapper.Map<PlaylistArtistDto>(artist);

            return Ok(_linkService.CreateLinksWhenDeleted(mappedArtist));
        }
    }
}
