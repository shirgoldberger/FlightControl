using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        public int Id { get; set; }
        public int Passengers { get; set; }
        public string Company_name { get; set; }
        public location Initial_location { get; set; }
        public List<segment> Segments { get; set; }
    }
}