using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SpotifyApi.Domain.Dtos;
using SpotifyApi.Domain.EntityModels;
using SpotifyApi.Domain.Logic.AuxServicies.IAuxServicies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Logic.AuxServicies
{
    public class SwaggerUiService : IAuxUserAgentService
    {
        private IConfiguration _config;
        private const string SWAGGER_BASE_URL = "https://api.whatismybrowser.com/api/v2/";  

        public SwaggerUiService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<UserAgent> ParseUserAgentData(string userAgentData)
        {
            var apiAddress = new Uri(SWAGGER_BASE_URL + "user_agent_parse");
            HttpClient httpClient = new HttpClient();
         

            //add x-api-key and client id for request
            httpClient.DefaultRequestHeaders.Add("x-api-key", _config.GetSection("AppSettings:SwaggerApiKey").Value);

            //constructing body of the post
            var jsonBody = JsonConvert.SerializeObject(new { user_agent = userAgentData });

            var content = new StringContent(jsonBody);

            var request = await httpClient.PostAsync(apiAddress, content);
            
            var c = await request.Content.ReadAsStringAsync();

            dynamic res = JsonConvert.DeserializeObject(c);

            try
            {
                //returning and constructing user agent from body
                return new UserAgent
                {
                    UserAgentDescription = res.parse.user_agent,
                    SoftwareName = res.parse.software_name,
                    OperatingSystem = res.parse.operating_system,
                    SimpleSubDescription = res.parse.simple_sub_description_string,
                    OperatingSystemName = res.parse.operating_system_name,
                    OperatingSystemVersion = res.parse.operating_system_version,
                    SimpleSoftware = res.parse.simple_software_string,
                    Software = res.parse.software,
                };
            } catch(Exception e)
            {
                return null;
            }
            
        }
    }
}
