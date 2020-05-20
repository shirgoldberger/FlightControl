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
        private readonly FlightDbContext _context;

        public FlightPlanController(FlightDbContext context)
        {
            _context = context;
        }

        // GET: api/FlightPlan
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightPlan>>> GetFlightPlan()
        {
            var loc = await _context.firstLoc.ToListAsync();
            var seg = await _context.segments.ToListAsync();
            List<FlightPlan> fp = await _context.flightPlan.ToListAsync();
            foreach (FlightPlan element in fp)
            {
                string id = element.Id;
                element.Initial_location = loc.Where(a => a.Id.CompareTo(id) == 0).First();
                element.Segments = seg.Where(a => a.Id.CompareTo(id) == 0).ToList();
            }
            return fp;
        }

        // GET: api/FlightPlan/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlan>> GetFlightPlan(string id)
        {
            var loc = await _context.firstLoc.ToListAsync();
            var seg = await _context.segments.ToListAsync();
            var flightPlan = await _context.flightPlan.FindAsync(id);
            if (flightPlan != null)
            {
                flightPlan.Initial_location = loc.Where(a => a.Id.CompareTo(id) == 0).First();
                flightPlan.Segments = seg.Where(a => a.Id.CompareTo(id) == 0).ToList();
                return flightPlan;
            }
            // Ask only the relevant server.
            try
            {
                var s = FlightDbContext.serverId[id];
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
            if (id.CompareTo(flightPlan.Id) != 0)
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

        // POST: api/FlightPlan
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FlightPlan>> PostFlightPlan(FlightPlan flightPlan)
        {
            // Set ID.
            flightPlan.Id = IDGenerator();
            _context.flightPlan.Add(flightPlan);
            // Create flight with the relevent flight id. *** the flight id is placed just when adding it to the DataBase.
            var loc = flightPlan.Initial_location;
            loc.Id = flightPlan.Id;
            _context.firstLoc.Add(loc);
            var seg = flightPlan.Segments;
            foreach (Segment element in seg)
            {
                element.Id = flightPlan.Id;
                _context.segments.Add(element);
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFlightPlan", new { id = flightPlan.Id }, flightPlan);
        }

        // DELETE: api/FlightPlan/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FlightPlan>> DeleteFlightPlan(string id)
        {
            var loc = await _context.firstLoc.ToListAsync();
            var seg = await _context.segments.ToListAsync();
            var flightPlan = await _context.flightPlan.FindAsync(id);
            if (flightPlan == null)
            {
                return NotFound();
            }
            var first_loc = loc.Where(a => a.Id.CompareTo(id) == 0).First();
            var segments = seg.Where(a => a.Id.CompareTo(id) == 0).ToList();
            _context.firstLoc.Remove(first_loc);
            foreach (Segment element in segments)
            {
                _context.segments.Remove(element);
            }
            _context.flightPlan.Remove(flightPlan);

            await _context.SaveChangesAsync();

            return flightPlan;
        }

        private bool FlightPlanExists(string id)
        {
            return _context.flightPlan.Any(e => e.Id.CompareTo(id) == 0);
        }
        public string IDGenerator()
        {
            // Create random ID that look like- 'AA-00000000'.
            string id = "";
            // Generates a key.
            char c1 = LocalLibrary.getLetter();
            id = id + c1;
            char c2 = LocalLibrary.getLetter();
            id = id + c2;
            id = id + "-";
            // Generates the numbers.
            int num1 = LocalLibrary.getNumber();
            id = id + num1;
            int num2 = LocalLibrary.getNumber();
            id = id + num2;
            int num3 = LocalLibrary.getNumber();
            id = id + num3;
            int num4 = LocalLibrary.getNumber();
            id = id + num4;
            int num5 = LocalLibrary.getNumber();
            id = id + num5;
            int num6 = LocalLibrary.getNumber();
            id = id + num6;
            int num7 = LocalLibrary.getNumber();
            id = id + num7;
            int num8 = LocalLibrary.getNumber();
            id = id + num8;
            return id;
        }
    }
}