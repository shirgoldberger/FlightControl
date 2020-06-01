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

        public int Key { get; set; }
        [JsonIgnore]

        public string ID { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string DateTime { get; set; }
    }
}
