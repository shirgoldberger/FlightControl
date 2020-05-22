using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FlightControlWeb.Models
{
    public class Location
    {
        [Key]
        [JsonIgnore]

        public int key { get; set; }
        [JsonIgnore]

        public string Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string date_time { get; set; }
    }
}
