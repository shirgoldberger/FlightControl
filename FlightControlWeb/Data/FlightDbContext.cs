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
        public DbSet<FlightPlan> flightPlan { get; set; }
        public DbSet<location> firstLoc { get; set; }
        public DbSet<segment> segments { get; set; }
        public DbSet<Server> Server { get; set; }

        public static Dictionary<string, Server> serverId  = new Dictionary<string, Server>();

    }
}
