using AutoMapper;
using SpotifyApi.Domain.EntityModels;
using SpotifyApi.Domain.Models;

namespace SpotifyApi.Domain.Dtos
{
    public class ProfileConfiguration : Profile
    {
        public ProfileConfiguration()
        {
            CreateMap<User, UserDto>()
                .ReverseMap();

            CreateMap<Album, AlbumDto>()
                .ForMember(d => d.Tracks, o => o.MapFrom( a => a.Tracks))
                .ReverseMap();

            CreateMap<Album, AlbumToCreateDto>()
             .ForMember(d => d.Tracks, o => o.MapFrom(a => a.Tracks))
             .ReverseMap();

            CreateMap<Track, TrackDto>()
                .ForMember(d => d.Artists, o => o.MapFrom( a => a.Artists ))
                .ReverseMap();
            
            CreateMap<Track, TrackToCreateDto>()
                .ForMember(d => d.Artists, o => o.MapFrom(a => a.Artists))
                .ReverseMap();

            CreateMap<Artist, ArtistDto>()
                .ReverseMap();

            CreateMap<Artist, ArtistToCreateDto>()
                .ReverseMap();

            CreateMap<PlaylistAlbum, PlaylistAlbumDto>()
                .ForMember(d => d.Tracks, o => o.MapFrom(a => a.Tracks))
                .ReverseMap();


            CreateMap<PlaylistAlbum, PlaylistAlbumToCreateDto>()
                .ForMember(d => d.Tracks, o => o.MapFrom(a => a.Tracks))
                .ReverseMap();

            CreateMap<PlaylistArtist, PlaylistArtistDto>()
                .ForMember(d => d.Tracks, o => o.MapFrom(a => a.Tracks))
                .ReverseMap();

            CreateMap<PlaylistArtist, PlaylistArtistToCreateDto>()
              .ForMember(d => d.Tracks, o => o.MapFrom(a => a.Tracks))
              .ReverseMap();

            CreateMap<PlaylistTrack, PlaylistTrackDto>()
                .ReverseMap();
            
            CreateMap<PlaylistTrack, PlaylistTrackToCreateDto>()
                .ReverseMap();

            CreateMap<Request, RequestDto>()
                .ReverseMap();

            CreateMap<User, UserToReturnDto>()
                .ReverseMap();
            
            CreateMap<User, UserForLoginDto>()
                .ReverseMap();

            CreateMap<User, UserForRegisterDto>()
                .ReverseMap();
        }
    }
}
