using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Dtos
{
    public abstract class LinkedResourceBaseDto
    {
        public List<Link> Links { get; set; } = new List<Link>();
    }
}
