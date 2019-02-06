using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyApi.Domain.Models;
using SpotifyApi.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AutoMapper;
using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Logic.Links;
using System.Linq;
using SpotifyApi.Domain.Dtos.ResourceParameters;

namespace SpotifyApi.Controllers
{

    [Authorize(AuthenticationSchemes =
        JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private readonly IArtistRepo _artistRepo;
        private readonly IMapper _mapper;
        private readonly ILinkService<ArtistDto, ArtistResourceParameters> _linkService;

 
        public ArtistController(IArtistRepo artistRepo,
            IMapper mapper,
            ILinkService<ArtistDto, ArtistResourceParameters> linkService)
        {
            _artistRepo = artistRepo;
            _mapper = mapper;
            _linkService = linkService;
        }

        // GET: api/Artists
        [HttpGet(Name = "GetArtists")]
        public async Task<IActionResult> Get([FromQuery] ArtistResourceParameters resourceParameters)
        {
            var artists = _artistRepo.GetAllPaginationAsync(resourceParameters);
            var mappedArtists = _mapper.Map<IEnumerable<ArtistDto>>(artists);

            //constructing links to previus next page
            var previousPage = artists.HasPrevious ?
              _linkService.CreateResourceUri(resourceParameters, ResourceType.PreviousPage) : null;

            var nextPage = artists.HasNext ?
                _linkService.CreateResourceUri(resourceParameters, ResourceType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = artists.TotalCount,
                pageSize = artists.PageSize,
                currentPage = artists.CurrentPage,
                totalPages = artists.TotalPages,
                previousPageLink = previousPage,
                nextPageLink = nextPage
            };

            mappedArtists = mappedArtists.Select(artist =>
            {
                artist = _linkService.CreateLinks(artist);
                return artist;
            });
            
            return Ok(new {
                Values = mappedArtists,
                Links = paginationMetadata
            });
        }

        // POST: api/Artists
        [HttpPost(Name = "CreateArtists")]
        public async Task<IActionResult> Post([FromBody] ArtistDto artistDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var artist = _mapper.Map<Artist>(artistDto);

            _artistRepo.Add(artist);

            await _artistRepo.SaveChangesAsync();

            var mappedArtist = _mapper.Map<ArtistDto>(artist);

            return Ok(_linkService.CreateLinks(mappedArtist));
        }

        //get an artist by id
        [HttpGet("{id}", Name = "GetArtistById")]
        public async Task<IActionResult> Get(int id)
        {

            var artist = await _artistRepo.GetByIdAsync(id);

            if(artist == null)
            {
                return NotFound();
            }

            var mappedArtist = _mapper.Map<ArtistDto>(artist);

            return Ok(_linkService.CreateLinks(mappedArtist));

        }


        //delete a specific artists
        [HttpDelete("{id}", Name = "DeleteArtist")]
        public async Task<IActionResult> Delete(int id)
        {
            var artist = await _artistRepo.GetByIdAsync(id);

            if(artist == null)
            {
                return NotFound();
            }

            _artistRepo.Delete(artist);

            await _artistRepo.SaveChangesAsync();

            var mappedArtist = _mapper.Map<ArtistDto>(artist);

            return Ok(_linkService.CreateLinks(mappedArtist));

        }


        //update a specific artist
        [HttpPut("{id}", Name = "UpdateArtist")]
        public async Task<IActionResult> Update(int id, [FromBody] ArtistDto artistDto)
        {

            var mappedArtist = _mapper.Map<Artist>(artistDto);

            _artistRepo.Update(id, mappedArtist);

            await _artistRepo.SaveChangesAsync();

            var updatedArtist = await _artistRepo.GetByIdAsync(id);

            var mappedUpdatedArtist = _mapper.Map<ArtistDto>(updatedArtist);

            return Ok(_linkService.CreateLinks(mappedUpdatedArtist));
        }

    }
}
