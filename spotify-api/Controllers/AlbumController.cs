using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyApi.Domain.Models;
using SpotifyApi.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AutoMapper;
using SpotifyApi.Domain.Dtos;
using System.Collections.Generic;
using SpotifyApi.Domain.Logic.Links;
using System.Linq;
using SpotifyApi.Domain.Dtos.ResourceParameters;

namespace SpotifyApi.Controllers
{

    [Authorize(AuthenticationSchemes =
        JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly IAlbmRepo _albumRepo;
        private readonly IMapper _mapper;
        private readonly ILinkService<AlbumDto, AlbumResourceParameters> _albumLinkService;
    

        public AlbumController(IAlbmRepo albumRepo, 
            IMapper mapper,
            ILinkService<AlbumDto, AlbumResourceParameters> albumLinkService)
        {
            _albumRepo = albumRepo;
            _mapper = mapper;
            _albumLinkService = albumLinkService;
        }

        
        /// <summary>
        /// Gets a paged list of all Albums
        /// </summary>
        /// <remarks>
        /// Sample header:
        /// Authentication: Bearer {token}
        /// Sample request:
        ///
        ///     GET /api/album
        ///
        /// </remarks>
        /// <returns>A  paged list of Albums</returns>
        /// <response code="200"></response>  
        [HttpGet(Name = "GetAlbums")]
        public async Task<IActionResult> Get([FromQuery] AlbumResourceParameters resourceParameters)
        {
            var albums = _albumRepo.GetAllPagination(resourceParameters);
            var mappedAlbums = _mapper.Map<IEnumerable<AlbumDto>>(albums);

            //Construct links to previous+ next page
            var previousPage = albums.HasPrevious ?
                _albumLinkService.CreateResourceUri(resourceParameters, ResourceType.PreviousPage) : null;

            var nextPage = albums.HasNext ?
                _albumLinkService.CreateResourceUri(resourceParameters, ResourceType.NextPage) : null;

            mappedAlbums = mappedAlbums.Select(album =>
            {
                album = _albumLinkService.CreateLinks(album);

                return album;
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
        /// Gets a specific Album by {id}
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/album/{id}
        ///     {
        ///        "id": 1,
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Required</param>
        /// <returns>The album with the given id</returns>
        /// <response code="200">Returns the album</response>
        /// <response code="400">If the request has no id</response>   
        /// <response code="404">Album with given id not found</response>  
        [HttpGet("{id}", Name = "GetAlbumById")]
        public async Task<IActionResult> Get(int id)
        {
            var album = await _albumRepo.GetByIdAsync(id);

            if (album == null)
            {
                return NotFound();
            }

            var mappedAlbum = _mapper.Map<AlbumDto>(album);

            return Ok(_albumLinkService.CreateLinks(mappedAlbum));
        }


        /// <summary>
        /// Creates a specific Album 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/album
        ///     {
        ///        "name": "Best Album",
        ///        "type": "Rap",
        ///        "imgUri": "http://album.com/album_photo",
        ///        "tracks": []
        ///     }
        ///
        /// </remarks>
        /// <returns>The album </returns>
        /// <response code="200">Returns the created album</response>
        /// <response code="400">Invalid model</response>   
        [HttpPost(Name = "CreateAlbum")]
        public async Task<IActionResult> Post([FromBody] AlbumToCreateDto albumDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            //mapping dto to entity
            var album = _mapper.Map<Album>(albumDto);

            _albumRepo.Add(album);

            await _albumRepo.SaveChangesAsync();

            var mappedAlbum = _mapper.Map<AlbumDto>(album);
            
            return Ok(_albumLinkService.CreateLinks(mappedAlbum));
        }


        /// <summary>
        /// Deletes a specific album
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/album/{id}
        ///     {
        ///        "id": 1,
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Required</param>
        /// <returns>The album with the given id</returns>
        /// <response code="200">Returns deleted album</response>
        /// <response code="400">If the request has no id</response>   
        /// <response code="404">Album with given id not found</response>  
        [HttpDelete("{id}", Name = "DeleteAlbum")]
        public async Task<IActionResult> Delete(int id)
        {
            var album = await _albumRepo.GetByIdAsync(id);

            if (album == null)
            {
                return NotFound();
            }

            _albumRepo.Delete(album);

            await _albumRepo.SaveChangesAsync();

            var mappedAlbum = _mapper.Map<AlbumDto>(album);

            return Ok(_albumLinkService.CreateLinksWhenDeleted(mappedAlbum));
        }

        /// <summary>
        /// Updates a specific Album by {id}
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/album/{id}
        ///     {  
        ///        "name": "Best Album",
        ///        "type": "Rap",
        ///        "imgUri": "http://album.com/album_photo",
        ///        "tracks": []
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Required</param>
        /// <returns>The album with the given id</returns>
        /// <response code="200">Returns the album</response>
        /// <response code="400">If the request has no id or invalid album model</response>   
        /// <response code="404">Album with given id not found</response>  
        [HttpPut("{id}", Name = "UpdateAlbum")]
        public async Task<IActionResult> Update(int id, [FromBody] AlbumToCreateDto albumDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var album = _mapper.Map<Album>(albumDto);
            
            _albumRepo.Update(id, album);

            await _albumRepo.SaveChangesAsync();

            var updatedAlbum = await _albumRepo.GetByIdAsync(id);

            var mappedUpdatedAlbum = _mapper.Map<AlbumDto>(updatedAlbum);

            return Ok(_albumLinkService.CreateLinks(mappedUpdatedAlbum));
        }

        /// <summary>
        /// Adds a track to a specific album
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH /{id}/track
        ///     {  
        ///        "name": "Best track",
        ///        "href": "http://best_track.com/2",
        ///        "previewUrl": "http://best_track.com/2",
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Required</param>
        /// <returns>The album with the given id</returns>
        /// <response code="200">Returns the album with new track</response>
        /// <response code="400">If the request has no id or invalid track model</response>   
        /// <response code="404">Album with given id not found</response>  
        [HttpPatch("{id}/track", Name = "AddTrackToAlbum")]
        public async Task<IActionResult> AddTrack(int id, [FromBody] TrackDto trackDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var track = _mapper.Map<Track>(trackDto);

            await _albumRepo.AddTrackToAlbum(id, track);

            await _albumRepo.SaveChangesAsync();

            var album = await _albumRepo.GetByIdAsync(id);

            var mappedAlbum = _mapper.Map<AlbumDto>(album);

            return Ok(_albumLinkService.CreateLinks(mappedAlbum));
        }

    }
}
