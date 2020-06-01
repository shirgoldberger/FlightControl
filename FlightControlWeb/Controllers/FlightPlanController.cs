using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Data;
using FlightControlWeb.Models;
using System.Net;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        private readonly FlightDbContext context;

        public FlightPlanController(FlightDbContext c)
        {
            context = c;
        }


        // GET: api/FlightPlan/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlan>> GetFlightPlan(string id)
        {
            var loc = await context.FirstLoc.ToListAsync();
            var seg = await context.Segments.ToListAsync();
            var flightPlan = await context.FlightPlan.FindAsync(id);
            if (flightPlan != null)
            {
                flightPlan.InitialLocation = loc.Where(a => a.ID.CompareTo(id) == 0).First();
                flightPlan.Segments = seg.Where(a => a.ID.CompareTo(id) == 0).ToList();
                return flightPlan;
            }
            // Ask only the relevant server.
            try
            {
                var s = FlightDbContext.ServerID[id];
                if (s == null)
                {
                    return NotFound();
                }
                string get = s.ServerURL + "api/FlightPlan/" + id;
                flightPlan = LocalLibrary.GetFlightFromServer<FlightPlan>(get);
                if (flightPlan == null)
                {
                    return NotFound();
                }
            }
            catch (WebException)
            {
                return NotFound();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return flightPlan;
        }

        // PUT: api/FlightPlan/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlightPlan(string id, FlightPlan flightPlan)
        {
            if (id.CompareTo(flightPlan.ID) != 0)
            {
                return BadRequest();
            }

            context.Entry(flightPlan).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightPlanExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FlightPlan
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FlightPlan>> PostFlightPlan(FlightPlan flightPlan)
        {
            // Set ID.
            flightPlan.ID = IDGenerator();
            context.FlightPlan.Add(flightPlan);
            // Create flight with the relevent flight id. *** the flight id is placed just when adding it to the DataBase.
            var loc = flightPlan.InitialLocation;
            loc.ID = flightPlan.ID;
            context.FirstLoc.Add(loc);
            var seg = flightPlan.Segments;
            foreach (Segment element in seg)
            {
                element.ID = flightPlan.ID;
                context.Segments.Add(element);
            }
            await context.SaveChangesAsync();

            return CreatedAtAction("GetFlightPlan", new { id = flightPlan.ID }, flightPlan);
        }

        // DELETE: api/FlightPlan/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FlightPlan>> DeleteFlightPlan(string id)
        {
            var loc = await context.FirstLoc.ToListAsync();
            var seg = await context.Segments.ToListAsync();
            var flightPlan = await context.FlightPlan.FindAsync(id);
            if (flightPlan == null)
            {
                return NotFound();
            }
            var first_loc = loc.Where(a => a.ID.CompareTo(id) == 0).First();
            var segments = seg.Where(a => a.ID.CompareTo(id) == 0).ToList();
            context.FirstLoc.Remove(first_loc);
            foreach (Segment element in segments)
            {
                context.Segments.Remove(element);
            }
            context.FlightPlan.Remove(flightPlan);
            await context.SaveChangesAsync();
            return flightPlan;
        }

        private bool FlightPlanExists(string id)
        {
            return context.FlightPlan.Any(e => e.ID.CompareTo(id) == 0);
        }
        public string IDGenerator()
        {
            // Create random ID that look like- 'AA-00000000'.
            string id = "";
            // Generates a key.
            char c1 = LocalLibrary.GetLetter();
            id = id + c1;
            char c2 = LocalLibrary.GetLetter();
            id = id + c2;
            id = id + "-";
            // Generates the numbers.
            int num1 = LocalLibrary.GetNumber();
            id = id + num1;
            int num2 = LocalLibrary.GetNumber();
            id = id + num2;
            int num3 = LocalLibrary.GetNumber();
            id = id + num3;
            int num4 = LocalLibrary.GetNumber();
            id = id + num4;
            int num5 = LocalLibrary.GetNumber();
            id = id + num5;
            int num6 = LocalLibrary.GetNumber();
            id = id + num6;
            int num7 = LocalLibrary.GetNumber();
            id = id + num7;
            int num8 = LocalLibrary.GetNumber();
            id = id + num8;
            return id;
        }
    }
}