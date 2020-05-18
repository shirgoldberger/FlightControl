using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Server
    {
        [Key]
        [JsonPropertyName("ServerId")]

        public int ServerId { get; set; }
        [JsonPropertyName("ServerURL")]

        public string ServerURL { get; set; }
    }
}
