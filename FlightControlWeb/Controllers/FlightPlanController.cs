﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Data;
using FlightControlWeb.Models;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using Newtonsoft.Json;

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
            if (flightPlan == null)
            {
                //ASK ONLY THE RELEVANT SERVER
                try
                {
                    var s = FlightDbContext.serverId[id];
                    if (s == null)
                    {
                        return NotFound();
                    }
                    string get = s.ServerURL + "api/FlightPlan/" + id;
                    flightPlan = GetFlightFromSever<FlightPlan>(get);
                    if (flightPlan == null)
                    {
                        return NotFound();
                    }

                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }


            }
            else
            {
                flightPlan.Initial_location = loc.Where(a => a.Id.CompareTo(id) == 0).First();
                flightPlan.Segments = seg.Where(a => a.Id.CompareTo(id) == 0).ToList();
            }

            return flightPlan;
        }

        // PUT: api/FlightPlan/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlightPlan(string id, FlightPlan flightPlan)
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

        // POST: api/FlightPlan
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FlightPlan>> PostFlightPlan(FlightPlan flightPlan)
        {

            //SET ID
            flightPlan.Id = IDGenerator();
            _context.flightPlan.Add(flightPlan);
            //create flight with the relevent flight id. *** the flight id is placed just when adding it to the DataBase
            var loc = flightPlan.Initial_location;
            loc.Id = flightPlan.Id;
            _context.firstLoc.Add(loc);
            var seg = flightPlan.Segments;
            foreach (segment element in seg)
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
            foreach (segment element in segments)
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
            string id = "";
            // generates a key
            char c1 = getLetter();
            id = id + c1;
            char c2 = getLetter();
            id = id + c2;
            id = id + "-";
            // generates the numbers
            int num1 = getNumber();
            id = id + num1;
            int num2 = getNumber();
            id = id + num2;
            int num3 = getNumber();
            id = id + num3;
            int num4 = getNumber();
            id = id + num4;
            int num5 = getNumber();
            id = id + num5;
            int num6 = getNumber();
            id = id + num6;
            int num7 = getNumber();
            id = id + num7;
            int num8 = getNumber();
            id = id + num8;
            return id;
        }

        public char getLetter()
        {
            var rand = new Random();
            int num = rand.Next(0, 26);
            char letter = (char)('A' + num);
            return letter;
        }

        public int getNumber()
        {
            var rand = new Random();
            int num = rand.Next(0, 10);
            return num;
        }

        public static T GetFlightFromSever<T>(string serverUrl)
        {
            string get = String.Format(serverUrl);
            WebRequest request = WebRequest.Create(get);
            request.Method = "GET";
            HttpWebResponse response = null;
            response = (HttpWebResponse)request.GetResponse();
            string result = null;
            // get data
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                result = sr.ReadToEnd();
                sr.Close();
            }
            if (result == "" || result == null)
            {
                return default;
            }
            T externalFlights = JsonConvert.DeserializeObject<T>(result);
            return externalFlights;
        }
    }
}