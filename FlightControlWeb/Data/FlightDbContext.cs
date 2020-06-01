using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;

namespace FlightControlWeb.Data
{
    public class FlightDbContext : DbContext
    {
        public FlightDbContext (DbContextOptions<FlightDbContext> options)
            : base(options)
        {
        }
        public DbSet<FlightPlan> FlightPlan { get; set; }
        public DbSet<Location> FirstLoc { get; set; }
        public DbSet<Segment> Segments { get; set; }
        public DbSet<Server> Server { get; set; }

        public static Dictionary<string, Server> ServerID  = new Dictionary<string, Server>();

    }
}
