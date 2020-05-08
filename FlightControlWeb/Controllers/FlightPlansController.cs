using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;
using FlightControlWeb.Data;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlansController : ControllerBase
    {
        private readonly FlightDbContext _context;

        public FlightPlansController(FlightDbContext context)
        {
            _context = context;
        }

        // GET: api/FlightPlans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightPlan>>> GetFlightPlan()
        {
            List<FlightPlan> fp = await _context.flightPlan.ToListAsync();
            foreach (FlightPlan element in fp)
            {
                string id = element.Flight_ID;
                var loc = await _context.firstLoc.ToListAsync();
                var seg = await _context.segments.ToListAsync();

                element.Initial_location = loc.Where(a => !a.Id.Equals(id)).First();
                element.Segments = seg.Where(a => !a.Id.Equals(id)).ToList();
                return fp;
            }
            return fp;
        }

        // GET: api/FlightPlans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlan>> GetFlightPlan(int id)
        {
            var flightPlan = await _context.flightPlan.FindAsync(id);

            if (flightPlan == null)
            {
                return NotFound();
            }

            return flightPlan;
        }

        // PUT: api/FlightPlans/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlightPlan(int id, FlightPlan flightPlan)
        {
            if (id != flightPlan.Id)
            {
                return BadRequest();
            }

            _context.Entry(flightPlan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

        // POST: api/FlightPlans
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FlightPlan>> PostFlightPlan(FlightPlan flightPlan)
        {
            //create flight with the relevent flight id.
            //SET ID
            flightPlan.Flight_ID = "LS - 1234353463";
            _context.flightPlan.Add(flightPlan);
            var loc = flightPlan.Initial_location;
            loc.Id = flightPlan.Flight_ID;
            _context.firstLoc.Add(loc);
            var seg = flightPlan.Segments;
            foreach (segment element in seg)
            {
                element.Id = flightPlan.Flight_ID;
                _context.segments.Add(element);
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFlightPlan", new { id = flightPlan.Id }, flightPlan);
        }

        // DELETE: api/FlightPlans/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FlightPlan>> DeleteFlightPlan(int id)
        {
            var flightPlan = await _context.flightPlan.FindAsync(id);
            if (flightPlan == null)
            {
                return NotFound();
            }

            _context.flightPlan.Remove(flightPlan);
            await _context.SaveChangesAsync();

            return flightPlan;
        }

        private bool FlightPlanExists(int id)
        {
            return _context.flightPlan.Any(e => e.Id == id);
        }
    }
}
