using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        public int Id { get; set; }

        [JsonPropertyName("flight_id")]
        public string Flight_ID { get; set; }

        [JsonPropertyName("passengers")]
        public int Passengers { get; set; }
        [JsonPropertyName("company_name")]

        public string Company_name { get; set; }
        [NotMapped]
        [JsonPropertyName("initial_location")]

        public location Initial_location { get; set; }
        [JsonPropertyName("segments")]

        [NotMapped]
        public List<segment> Segments { get; set; }
    }
}