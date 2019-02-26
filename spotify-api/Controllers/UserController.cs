using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpotifyApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.Logic;
using SpotifyApi.Domain;
using Microsoft.AspNetCore.Authorization;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using SpotifyApi.Domain.Logic.Links;
using SpotifyApi.Domain.Services.IRepos;

namespace SpotifyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IUserRepo _userRepo;
        private readonly ILinkService<UserDto, UserResourceParameters> _linkService;


        public UserController(IConfiguration config,
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            IMapper mapper,
            IUserRepo userRepo,
            ILinkService<UserDto, UserResourceParameters> linkService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _mapper = mapper;
            _userRepo = userRepo;
            _linkService = linkService;
        }

        /// <summary>
        /// Creates a token for a user 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/user/login
        ///     {
        ///        "userName": "Xulescu",
        ///        "password": "NotParola12"
        ///     }
        ///
        /// </remarks>
        /// <returns>The token and some user info </returns>
        /// <response code="200">Returns the created toen + user</response>
        /// <response code="400">Invalid model</response>   
        [HttpPost("login", Name = "LoginUser")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(userDto.UserName);

            if(user == null)
            {
                return Unauthorized();
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, userDto.Password, false);

            if(result.Succeeded)
            {

                //uncoment the section to add admins manually
                //add user role to user
                //await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Admin")); 

                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "User"));

                var mappedUserToDto = _mapper.Map<UserToReturnDto>(user);
                var userForLinks = _mapper.Map<UserDto>(user);

                return Ok(new
                {
                    token = await TokenGenerator.GenerateJwtToken(user, _mapper, _config, _userManager),
                    user = mappedUserToDto,           
                });
            }

            return Unauthorized();
        }


        /// <summary>
        /// Creates a new user with user permissions
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/user
        ///     {
        ///        "userName": "Xulescu",
        ///        "email": "x_ulescu@gmail.com",
        ///        "password": "NotParola12"
        ///     }
        ///
        /// </remarks>
        /// <returns>No Content</returns>
        /// <response code="201">No Content</response>
        /// <response code="400">Invalid model</response>
        [HttpPost(Name = "CreateUser")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            
            var result = await _userManager.CreateAsync(user, userDto.Password);
            
            if(result.Succeeded)
            {
                return StatusCode(201);
            }

            return BadRequest(result.Errors);
        }
        
        /// <summary>
        /// Creates a new user with user and admin permissions
        /// </summary>
        /// <remarks>
        /// Remark: You have to have admin permission to create an admin user
        /// Sample request:
        ///     
        ///     POST /api/user
        ///     {
        ///        "userName": "Xulescu",
        ///        "email": "x_ulescu@gmail.com",
        ///        "password": "NotParola12"
        ///     }
        ///
        /// </remarks>
        /// <returns>No Content</returns>
        /// <response code="201">No Content</response>
        /// <response code="400">Invalid model</response>
        [HttpPost("admin", Name = "CreateAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] UserForRegisterDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                //get user by name
                var adminUser = await _userManager.FindByNameAsync(user.UserName);

                //if user not found throw exception
                if (adminUser == null)
                {
                    throw new NullReferenceException("Failed to create admin user");
                }

                //add admin role to user
                await _userManager.AddClaimAsync(adminUser, new Claim(ClaimTypes.Role, "Admin"));

                return StatusCode(201);
            }

            return BadRequest(result.Errors);
        }

        [HttpGet(Name = "GetUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get([FromQuery] UserResourceParameters resourceParameters)
        {
            var users = _userRepo.GetAllPagination(resourceParameters);
            var mappedUsers = _mapper.Map<IEnumerable<UserToReturnDto>>(users);

            //Construct links to previous+ next page
            var previousPage = users.HasPrevious ?
                _linkService.CreateResourceUri(resourceParameters, ResourceType.PreviousPage) : null;

            var nextPage = users.HasNext ?
                _linkService.CreateResourceUri(resourceParameters, ResourceType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = users.TotalCount,
                pageSize = users.PageSize,
                currentPage = users.CurrentPage,
                totalPages = users.TotalPages,
                previousPageLink = previousPage,
                nextPageLink = nextPage
            };

            return Ok(new
            {
                Values = mappedUsers,
                Links = paginationMetadata
            });
        }

    }
}
