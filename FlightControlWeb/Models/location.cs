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
        [JsonPropertyName("longitude")]

        public double Longitude { get; set; }
        [JsonPropertyName("latitude")]

        public double Latitude { get; set; }
        [JsonPropertyName("date_time")]

        public string DateTime { get; set; }
    }
}
