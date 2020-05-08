using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class location
    {
        [Key]
        public int key { get; set; }

        public string Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string date_time { get; set; }
    }
}