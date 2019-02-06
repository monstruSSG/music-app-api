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
    public class PlaylistAlbumController : ControllerBase
    {

        private readonly IPlaylistAlbumRepo _playlistAlbumRepo;
        private readonly IMapper _mapper;
        private readonly ILinkService<PlaylistAlbumDto, PlaylistAlbumResourceParameters> _linkService;

        public PlaylistAlbumController(IPlaylistAlbumRepo playlistAlbumRepo, 
            IMapper mapper,
            ILinkService<PlaylistAlbumDto, PlaylistAlbumResourceParameters> linkService)
        {
            _playlistAlbumRepo = playlistAlbumRepo;
            _mapper = mapper;
            _linkService = linkService;
        }

        [HttpGet(Name = "GetPlaylistAlbums")]
        public async Task<IActionResult> Get([FromQuery] PlaylistAlbumResourceParameters resourceParameters)
        {
            var albums = _playlistAlbumRepo.GetAllPaginationAsync(resourceParameters);

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

        [HttpPost(Name = "CreatePlaylistAlbum")]
        public async Task<IActionResult> Post([FromBody] PlaylistAlbumDto albumDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var album = _mapper.Map<PlaylistAlbum>(albumDto);

            _playlistAlbumRepo.Add(album);

            await _playlistAlbumRepo.SaveChangesAsync();

            var mappedAlbum = _mapper.Map<PlaylistAlbumDto>(album);

            return Ok(_linkService.CreateLinks(mappedAlbum));
        }

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

            
            return StatusCode(204);
        }


    }
}
