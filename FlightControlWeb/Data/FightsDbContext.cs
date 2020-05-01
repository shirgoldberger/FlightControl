using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;

namespace FlightControlWeb.Data
{
    public class FightsDbContext : DbContext
    {
        public FightsDbContext (DbContextOptions<FightsDbContext> options)
            : base(options)
        {
        }

        public DbSet<FlightControlWeb.Models.Flight> Flight { get; set; }
    }
}
