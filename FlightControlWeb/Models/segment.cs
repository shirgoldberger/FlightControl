using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FlightControlWeb.Models
{
    public class Segment
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
        [JsonPropertyName("timespan_seconds")]

        public int TimespanSeconds { get; set; }
    }
}
