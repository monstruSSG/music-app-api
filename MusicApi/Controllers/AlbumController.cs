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


        [HttpGet(Name = "GetAlbums")]
        public async Task<IActionResult> Get([FromQuery] AlbumResourceParameters resourceParameters)
        {
            var a = _albumRepo;
            var albums = _albumRepo.GetAllPaginationAsync(resourceParameters);
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

        [HttpPost(Name = "CreateAlbum")]
        public async Task<IActionResult> Post([FromBody] AlbumDto albumDto)
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

            return Ok(_albumLinkService.CreateLinks(mappedAlbum));
        }


        [HttpPut("{id}", Name = "UpdateAlbum")]
        public async Task<IActionResult> Update(int id, [FromBody] AlbumDto albumDto)
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
