using FlightControlWeb.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb
{
    public class test
    {
        public test()
        {
            string[] args = { "" };
            var h = Program.CreateHostBuilder(args);
            var d = new DbContextOptionsBuilder<FlightDbContext>();
            d.UseInMemoryDatabase("moshe");
            FlightDbContext dbContext = new FlightDbContext(d.Options);


        }
    }
}
