﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Data;
using FlightControlWeb.Models;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly FlightDbContext _context;

        public FlightsController(FlightDbContext context)
        {
            _context = context;
        }

        // GET: api/Flights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlight([FromQuery] string relative_to = null)
        {
            if (relative_to == null)
            {
                return NotFound();
            }
            // True if the request contain 'sync_all'.
            Boolean sync = Request.Query.ContainsKey("sync_all");
            FlightDbContext.serverId.Clear();
            _context.SaveChanges();
            List<Flight> flights = new List<Flight>();
            DateTime relative = new DateTime();
            try
            {
                relative = LocalLibrary.ConvertToDateTime(relative_to);
            }
            catch (Exception)
            {
                // Relative_to is not a valid time.
                return NotFound();
            }
            // Take internal flights.
            List<Flight> internalFlights = CreateInternalFlights(relative);
            foreach (Flight inf in internalFlights)
            {
                flights.Add(inf);
            }
            // Take external flights if the request contain 'sync_all'.
            if (sync)
            {
                List<Flight> externalFlights = CreateExternalFlights(relative_to);
                foreach(Flight exf in externalFlights)
                {
                    flights.Add(exf);
                }
            }
            Task<ActionResult<IEnumerable<Flight>>> f = Task<ActionResult<IEnumerable<Flight>>>.Factory.StartNew(() =>
            {
                return flights;
            });
            return await f;
        }
        private List<Flight> CreateInternalFlights(DateTime relative)
        {
            List<Flight> flights = new List<Flight>();
            string time = relative.ToString("dd/MM/yyyy HH:mm:ss");
            // run over FilghtPlans.
            foreach (FlightPlan fp in _context.flightPlan)
            {
                string id = fp.Id;
                fp.Initial_location = TakeInitialLocation(id).Result;
                fp.Segments = TakeSegments(id).Result;
                DateTime start = LocalLibrary.ConvertToDateTime(fp.Initial_location.Date_time);
                if (DateTime.Compare(relative, start) < 0)
                {
                    continue;
                }
                Flight f = new Flight();
                if (CheckSegments(fp, f, start, relative))
                {
                    flights.Add(f);
                }
                // Save other properties.
                f.Flight_id = fp.Id;
                f.Passengers = fp.Passengers;
                f.Company_name = fp.Company_name;
                f.Date_time = time;
                f.Is_external = false;
            }
            return flights;
        }
        private List<Flight> CreateExternalFlights(string relative_to)
        {
            List<Flight> externalFlights = new List<Flight>();
            foreach (Server s in _context.Server)
            {
                string get = s.ServerURL + "api/Flights?relative_to=" + relative_to;

                try
                {
                    externalFlights = LocalLibrary.GetFlightFromServer<List<Flight>>(get);

                }
                catch (Exception)
                {
                    continue;
                }
                foreach (Flight f in externalFlights)
                {
                    f.Is_external = true;
                    // Save the server that the current flight belongs to him.
                    FlightDbContext.serverId[f.Flight_id] = s;
                }
            }
            _context.SaveChanges();
            return externalFlights;
        }
        private async Task<Location> TakeInitialLocation(string id)
        {
            // Create location according to the given id.
            var loc = await _context.firstLoc.ToListAsync();
            Task<Location> l = Task<Location>.Factory.StartNew(() =>
            {
                return loc.Where(a => a.Id.CompareTo(id) == 0).First();
            });
            return await l;
        }
        private async Task<List<Segment>> TakeSegments(string id)
        {
            // Create segments according to the given id.
            var seg = await _context.segments.ToListAsync();
            Task<List<Segment>> s = Task<List<Segment>>.Factory.StartNew(() =>
            {
                return seg.Where(a => a.Id.CompareTo(id) == 0).ToList();
            });
            return await s;
        }
        private bool CheckSegments(FlightPlan fp, Flight f, DateTime start, DateTime relative)
        {
            bool isRelevant = false;
            double startLat = fp.Initial_location.Latitude;
            double startLong = fp.Initial_location.Longitude;
            Segment[] sortArray = fp.Segments.OrderBy(o => o.key).ToArray();
            double totalSeconds = (relative - start).TotalSeconds;
            double timePassed = 0;
            int segNum;
            // Run over the segments.
            for(segNum = 0; segNum < sortArray.Length; segNum++)
            {
                double time = totalSeconds - timePassed;
                // The plan is in this segment at the time is given.
                if (time < sortArray[segNum].timespan_seconds)
                {
                    double l = time / sortArray[segNum].timespan_seconds;
                    f.Latitude = startLat + l * (sortArray[segNum].Latitude - startLat);
                    f.Longitude = startLong + l * (sortArray[segNum].Longitude - startLong);
                    isRelevant = true;
                    break;
                }
                else
                {
                    // Save the start location of the segment.
                    startLat = sortArray[segNum].Latitude;
                    startLong = sortArray[segNum].Longitude;
                    timePassed += sortArray[segNum].timespan_seconds;
                }
            }
            return isRelevant;
        }

        // PUT: api/Flights/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlight(string id, Flight flight)
        {

            if (id.CompareTo(flight.Flight_id) != 0)
            {
                return BadRequest();
            }

            _context.Entry(flight).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(id))
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


        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FlightPlan>> DeleteFlight(string id)
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

        private bool FlightExists(string id)
        {
            return _context.flightPlan.Any(e => e.Id.CompareTo(id) == 0);
        }
    }
}
