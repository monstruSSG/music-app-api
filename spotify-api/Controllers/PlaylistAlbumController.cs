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
    public class PlaylistAlbumController : ControllerBase
    {

        private readonly IPlaylistAlbumRepo _playlistAlbumRepo;
        private readonly IMapper _mapper;
        private readonly ILinkService<PlaylistAlbumDto, PlaylistAlbumResourceParameters> _linkService;
        private readonly UserManager<User> _userManager;

        public PlaylistAlbumController(IPlaylistAlbumRepo playlistAlbumRepo,
            UserManager<User> userManager,
            IMapper mapper,
            ILinkService<PlaylistAlbumDto, PlaylistAlbumResourceParameters> linkService)
        {
            _playlistAlbumRepo = playlistAlbumRepo;
            _mapper = mapper;
            _linkService = linkService;
            _userManager = userManager;
        }

        /// <summary>
        /// Gets a paged list of all Albums
        /// </summary>
        /// <remarks>
        /// Sample header:
        /// Authentication: Bearer {token}
        /// Sample request:
        ///
        ///     GET /api/playlistalbum
        ///
        /// </remarks>
        /// <returns>A  paged list of Abums</returns>
        /// <response code="200"></response>  
        [HttpGet(Name = "GetPlaylistAlbums")]
        public async Task<IActionResult> Get([FromQuery] PlaylistAlbumResourceParameters resourceParameters)
        {
            var albums = _playlistAlbumRepo.GetAllPagination(resourceParameters);

            var mappedAlbums= _mapper.Map<IEnumerable<PlaylistAlbumDto>>(albums);

            //construct links to previus+next page
            var previousPage = albums.HasPrevious ?
               _linkService.CreateResourceUri(resourceParameters, ResourceType.PreviousPage) : null;

            var nextPage = albums.HasNext ?
                _linkService.CreateResourceUri(resourceParameters, ResourceType.NextPage) : null;

            mappedAlbums = mappedAlbums.Select(track =>
            {
                track = _linkService.CreateLinks(track);
                return track;
            });

            var paginationMetadata = new
            {
                totalCount = albums.TotalCount,
                pageSize = albums.PageSize,
                currentPage = albums.CurrentPage,
                totalPages = albums.TotalPages,
                previousPageLink = previousPage,
                nextPageLink = nextPage
            };

            return Ok(new
            {
                Values = mappedAlbums,
                Links = paginationMetadata
            });
        }

        /// <summary>
        /// Creates a specific Album 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/playlistalbum
        ///     {
        ///        "userName": "celMaiTareUSer",
        ///        "name": "Best Album",
        ///        "previewUrl": "http://listen.com/liste_me",
        ///        "href": "http://album.com/track",
        ///        "tracks": []
        ///     }
        ///
        /// </remarks>
        /// <returns>The Album </returns>
        /// <response code="200">Returns the created album</response>
        /// <response code="400">Invalid model</response> 
        [HttpPost(Name = "CreatePlaylistAlbum")]
        public async Task<IActionResult> Post([FromBody] PlaylistAlbumToCreateDto albumDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(albumDto.UserName);

            if (user == null)
            {
                return NotFound();
            }
            
            var album = _mapper.Map<PlaylistAlbum>(albumDto);

            if(_playlistAlbumRepo.GetByName(album.Name, user.UserName) == true)
            {
                return StatusCode(409);
            }

            _playlistAlbumRepo.Add(album);

            await _playlistAlbumRepo.SaveChangesAsync();

            var mappedAlbum = _mapper.Map<PlaylistAlbumDto>(album);

            return Ok(_linkService.CreateLinks(mappedAlbum));
        }


        /// <summary>
        /// Deletes a specific album
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/playlistalbum/{id}
        ///     {
        ///        "id": 1,
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Required</param>
        /// <returns>The album with the given id</returns>
        /// <response code="200">DeletedPlaylistAlbum</response>
        /// <response code="400">If the request has no id</response>   
        /// <response code="404">Album with given id not found</response> 
        [HttpDelete("{id}", Name = "DeletePlaylistAlbum")]
        public async Task<IActionResult> Delete(int id)
        {
            //get the album
            var album = await _playlistAlbumRepo.GetByIdAsync(id);

            if (album == null)
            {
                return BadRequest("Album with {id} not found");
            }

            //delete the album
            _playlistAlbumRepo.Delete(album);

            //save changes async
            await _playlistAlbumRepo.SaveChangesAsync();

            var mappedAlbum = _mapper.Map<PlaylistAlbumDto>(album);

            return Ok(_linkService.CreateLinksWhenDeleted(mappedAlbum));
        }


    }
}
