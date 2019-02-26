using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpotifyApi.Domain.Models;
using SpotifyApi.Domain.Models.Roles;
using SpotifyApi.Domain.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SpotifyApi.Domain.Dtos;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SpotifyApi.Domain.Logic.Links;
using SpotifyApi.Domain.Logic.Middleware;
using SpotifyApi.Domain.Logic.AuxServicies.IAuxServicies;
using SpotifyApi.Domain.Logic.AuxServicies;
using SpotifyApi.Domain.Dtos.ResourceParameters;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System.IO;
using System;
using Swashbuckle.AspNetCore.Filters;
using SpotifyApi.Domain.Services.IRepos;
using Microsoft.Extensions.FileProviders;

namespace SpotifyApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
           
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //configure services here
            services.AddScoped<IAlbmRepo,AlbumRepo>();
            services.AddScoped<IArtistRepo, ArtistRepo>();
            services.AddScoped<ITrackRepo, TrackRepo>();
            services.AddScoped<IPlaylistAlbumRepo, PlaylistAlbumRepo>();
            services.AddScoped<IPlaylistArtist, PlaylistArtistRepo>();
            services.AddScoped<IPlaylistTrackRepo, PlaylistTrackRepo>();
            services.AddScoped<IRequestRepo, RequestRepo>();
            services.AddScoped<IUserRepo, UserRepo>();

            //configuring services for links in controllers
            services.AddScoped<ILinkService<TrackDto, TrackResourceParameters>, TrackLinkService>();
            services.AddScoped<ILinkService<AlbumDto, AlbumResourceParameters>, AlbumLinkService>();
            services.AddScoped<ILinkService<ArtistDto, ArtistResourceParameters>, ArtistLinkService>();
            services.AddScoped<ILinkService<PlaylistAlbumDto, PlaylistAlbumResourceParameters>, PlaylistAlbumLinkService>();
            services.AddScoped<ILinkService<PlaylistArtistDto, PlaylistArtistResourceParameters>, PlaylistArtistLinkService>();
            services.AddScoped<ILinkService<PlaylistTrackDto, PlaylistTrackResourceParameters>, PlaylistTrackLinkService>();
            services.AddScoped<ILinkService<RequestDto, RequestResourceParameters>, RequestLinkService>();
            services.AddScoped<ILinkService<UserDto, UserResourceParameters>, UserLinkService>();

            //service for middleware user agent
            services.AddScoped<IAuxUserAgentService, UserAgentService>();

            //for constructing links
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory =>
            {
                var actionContext =
                    implementationFactory.GetService<IActionContextAccessor>().ActionContext;

                return new UrlHelper(actionContext);
            });
       
            services.AddCors();

            services.AddDbContext<DataContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //configure authorization
            IdentityBuilder builder = services.AddIdentityCore<User>(opt => 
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    });

            //end of identity configuration

            //configuring Automapper
            var config = new AutoMapper.MapperConfiguration(c =>
            {   
                c.AddProfile(new ProfileConfiguration());
            });

            var mapper = config.CreateMapper();


            //now add the mapper as a service, unique, global per application scope
            services.AddSingleton(mapper);

            services.AddSwaggerExamples();

            //add service for documentation using swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "SB Spotify Api :)",
                    Version = "v1",
                    Contact = new Contact
                    {
                        Name = "John Doe",
                        Email = "john@gmail.com",
                        Url = "https://twitter.com/john_doe"
                    },
                });

                c.ExampleFilters();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);


                // add Security information to each operation for OAuth2
                c.OperationFilter<SecurityRequirementsOperationFilter>();

                // if you're using the SecurityRequirementsOperationFilter, you also need to tell Swashbuckle you're using OAuth2
                c.AddSecurityDefinition("auth", new ApiKeyScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = "header",
                    Name = "Authorization",
                    Type = "apiKey"
                });
            });

            services.AddMvc(options =>
            {

            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            //configure static files to point to custom StaticContent
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                RequestPath = "/StaticContent",
            });

            //import it to serve generated swagger as JSON
            app.UseSwagger();

            //enable middleware to generate swagger-ui 
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SB Spotify Api");
                c.RoutePrefix = string.Empty;
            });

            app.UseMiddleware<RequestsObservatorMiddleware>();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
