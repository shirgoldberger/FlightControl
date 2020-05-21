using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        [JsonPropertyName("flight_id")]
        [Key]
        public string FlightPlan_id { get; set; }


        [JsonPropertyName("passengers")]
        public int Passengers { get; set; }
        [JsonPropertyName("company_name")]

        public string Company_name { get; set; }
        [NotMapped]
        [JsonPropertyName("initial_location")]

        public Location Initial_location { get; set; }
        [JsonPropertyName("segments")]

        [NotMapped]
        public List<Segment> Segments { get; set; }
    }
}
