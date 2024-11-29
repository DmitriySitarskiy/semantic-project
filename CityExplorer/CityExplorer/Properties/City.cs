using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityExplorer
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public int? Population { get; set; }
        public string? Description { get; set; }
        public string? History { get; set; }
        public string? ImageUrl { get; set; }
    }
}
