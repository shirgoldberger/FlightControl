using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        [Key]
        [JsonPropertyName("flight_id")]
        public string Flight_id { get; set; }
        [JsonPropertyName("longitude")]

        public double Longitude { get; set; }
        [JsonPropertyName("latitude")]

        public double Latitude { get; set; }
        [JsonPropertyName("passengers")]

        public int Passengers { get; set; }
        [JsonPropertyName("company_name")]

        public string Company_name { get; set; }
        [JsonPropertyName("date_time")]

        public string Date_time { get; set; }
        [JsonPropertyName("is_external")]

        public bool Is_external { get; set; }
    }
}
