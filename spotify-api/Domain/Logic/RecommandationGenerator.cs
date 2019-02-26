using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyApi.Domain.Logic
{
    public class RecommandationGenerator<T>
    {
        private List<T> _values;

        public RecommandationGenerator(List<T> values)
        {
            _values = values;
        }

        //"Recomands randomly 5 values if the list has so many or it is larger"
        public List<T> GetRecomandation()
        {
            Random rnd = new Random();
            List<T> recommandations = new List<T>();

            for (int i = 0; i < 5; i++ )
            {
                if(i <= _values.Count)
                {
                    recommandations.Add(_values[i]);
                } else
                {
                    break;
                }
            }

            return recommandations;
        }
    }
}
