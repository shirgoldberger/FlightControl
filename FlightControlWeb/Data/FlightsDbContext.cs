using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;

namespace FlightControlWeb.Data
{
    public class FlightsDbContext : DbContext
    {
        public FlightsDbContext (DbContextOptions<FlightsDbContext> options)
            : base(options)
        {
        }

        public DbSet<FlightControlWeb.Models.Flight> Flight { get; set; }

        public DbSet<FlightControlWeb.Models.FlightPlan> FlightPlan { get; set; }
    }
}
