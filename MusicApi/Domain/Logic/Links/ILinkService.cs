using SpotifyApi.Domain.Dtos;

namespace SpotifyApi.Domain.Logic.Links
{
    public interface ILinkService<T, Resource> 
        where T : LinkedResourceBaseDto
        where Resource : BaseResourceParameters
    {
        T CreateLinks(T t);
        string CreateResourceUri(Resource resourceParameters, ResourceType type);
    }
}
