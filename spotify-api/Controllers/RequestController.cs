using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using SpotifyApi.Domain.Logic.Links;
using SpotifyApi.Domain.Services;

namespace SpotifyApi.Controllers
{
    [Authorize(AuthenticationSchemes =
        JwtBearerDefaults.AuthenticationScheme,
        Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IRequestRepo _requestRepo;
        private readonly ILinkService<RequestDto, RequestResourceParameters> _linkService;
        private readonly IMapper _mapper;

        public RequestController(
            IRequestRepo requestRepo,
            ILinkService<RequestDto, RequestResourceParameters> linkService,
            IMapper mapper)
        {
            _requestRepo = requestRepo;
            _mapper = mapper;
            _linkService = linkService;
            
        }

        // GET: api/Request
        /// <summary>
        /// Gets a paged list of all Requests made to server
        /// </summary>
        /// <remarks>
        /// Sample header:
        /// Authentication: Bearer {token}
        /// Roles : Admin
        /// Sample request:
        ///
        ///     GET /api/request
        ///
        /// </remarks>
        /// <returns>A  paged list of Requests</returns>
        /// <response code="200"></response>  
        [HttpGet(Name = "GetRequests")]
        public async Task<IActionResult> Get([FromQuery] RequestResourceParameters resourceParameters)
        {

            //task: add dto and links to previous next apges
            var requests = _requestRepo.GetAllPagination(resourceParameters);

            //map requests to requestsDto
            var mappedRequests = _mapper.Map<IEnumerable<RequestDto>>(requests);

            //Construct links to previous+ next page
            var previousPage = requests.HasPrevious ?
                _linkService.CreateResourceUri(resourceParameters, ResourceType.PreviousPage) : null;

            var nextPage = requests.HasNext ?
                _linkService.CreateResourceUri(resourceParameters, ResourceType.NextPage) : null;

            //construct further links for every request
            mappedRequests = mappedRequests.Select(request =>
            {
                request = _linkService.CreateLinks(request);

                return request;
            });

            var paginationMetadata = new
            {
                totalCount = requests.TotalCount,
                pageSize = requests.PageSize,
                currentPage = requests.CurrentPage,
                totalPages = requests.TotalPages,
                previousPageLink = previousPage,
                nextPageLink = nextPage
            };
            
            return Ok(new
            {
                Values = mappedRequests,
                Metadata = paginationMetadata
            });
        }

        /// <summary>
        /// Gets a specific Request by {id}
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/request/{id}
        ///     {
        ///        "id": 1,
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Required</param>
        /// <returns>The Request with the given id</returns>
        /// <response code="200">Returns the Request</response>
        /// <response code="400">If the request has no id</response>   
        /// <response code="404">Request with given id not found</response> 
        [HttpGet("{id}",Name = "GetRequestById")]
        public async Task<IActionResult> Get(int id)
        {
            //get the request by id
            var request = await _requestRepo.GetByIdAsync(id);

            //map the requestobect -> dtorequest
            var mappedRequest = _mapper.Map<RequestDto>(request);

            return Ok(_linkService.CreateLinks(mappedRequest));
        }
        
    }
}
