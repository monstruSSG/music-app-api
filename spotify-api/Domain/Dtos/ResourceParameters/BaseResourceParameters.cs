using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Dtos
{
    public abstract class BaseResourceParameters
    {
        /// <summary>
        /// Enter a string and everiything that matches this string will be returned
        /// </summary>
        public string SearchQuery { get; set; }
        
        private const int maxPageSize = 20;

        /// <summary>
        /// Page number to be returned, default 1
        /// </summary>
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 5;


        /// <summary>
        /// Page size to be returned, default 10,
        /// if pageSize > 20 20 will be set as pageSize => maxium = 20
        /// </summary>
        public int PageSize
        {
            get
            {
                return _pageSize;
            }

            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
