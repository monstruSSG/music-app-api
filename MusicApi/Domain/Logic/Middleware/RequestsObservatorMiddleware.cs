using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SpotifyApi.Domain.EntityModels;
using SpotifyApi.Domain.Logic.AuxServicies.IAuxServicies;
using SpotifyApi.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Logic.Middleware
{
    public class RequestsObservatorMiddleware
    {
        private readonly RequestDelegate _next;
  

        public RequestsObservatorMiddleware(RequestDelegate next)
        {
            _next = next;
 
        }

        public async Task Invoke(HttpContext context, IRequestRepo requestRepo, IAuxUserAgentService userAgentService)
        {

            //getting user agent from header
            StringValues user_agent = "";
            context.Request.Headers.TryGetValue("User-Agent", out user_agent);


            //waiting for user agent
            var result = await userAgentService.ParseUserAgentData(user_agent.ToString());
 
            //saving request(request+useragentdata) data to dbcontext
            requestRepo.Add(new Request
            {
                Source = context.Connection.RemoteIpAddress.ToString() + ":" + context.Connection.RemotePort.ToString(),
                Destination = context.Request.Path.ToString(),
                Method = context.Request.Method,
                UserAgent = result,
            });

            //calling next to go to next middleware
            await _next(context);

        }
    }
}
