using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.Models
{
    public class Segment
    {
        [Key]
        public int key { get; set; }
        public string Id { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int timespan_seconds { get; set; }
    }
}
