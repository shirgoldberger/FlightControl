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
    public class FlightsController : ControllerBase
    {
        private readonly FlightDbContext _context;

        public FlightsController(FlightDbContext context)
        {
            _context = context;
        }

        private DateTime ConvertToDateTime(string str)
        {
            string pattern = @"(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})Z";
            if (Regex.IsMatch(str, pattern))
            {
                Match match = Regex.Match(str, pattern);
                int second = Convert.ToInt32(match.Groups[6].Value);
                int minute = Convert.ToInt32(match.Groups[5].Value);
                int hour = Convert.ToInt32(match.Groups[4].Value);
                int day = Convert.ToInt32(match.Groups[3].Value);
                int month = Convert.ToInt32(match.Groups[2].Value);
                int year = Convert.ToInt32(match.Groups[1].Value);
                return new DateTime(year, month, day, hour, minute, second);
            }
            else
            {
                throw new Exception("Unable to parse.");
            }
        }

        // GET: api/Flights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlight([FromQuery] string relative_to = null, [FromQuery] bool syncAll = false)
        {
            List<Flight> flights = new List<Flight>();
            if (relative_to != null)
            {
                DateTime relative = ConvertToDateTime(relative_to);
                // run over filghtPlans
                foreach (FlightPlan fp in _context.flightPlan)
                {
                    string id = fp.Id;
                    var loc = await _context.firstLoc.ToListAsync();
                    var seg = await _context.segments.ToListAsync();

                    fp.Initial_location = loc.Where(a => a.Id.CompareTo(id) == 0).First();
                    fp.Segments = seg.Where(a => a.Id.CompareTo(id) == 0).ToList();

                    DateTime start = ConvertToDateTime(fp.Initial_location.date_time);
                    if (DateTime.Compare(relative, start) < 0)
                    {
                        continue;
                    }
                    Flight f = new Flight();
                    f.Is_relevant = false;
                    double startLat = fp.Initial_location.Latitude;
                    double startLong = fp.Initial_location.Longitude;
                    // run over segments
                    foreach (segment s in fp.Segments)
                    {
                        DateTime saveStart = start;
                        DateTime test = start.AddSeconds(s.timespan_seconds);
                        if (DateTime.Compare(relative, start) >= 0 &&
                            DateTime.Compare(relative, test) <= 0)
                        {
                            f.Is_relevant = true;
                            TimeSpan difference = relative - saveStart;
                            double k = difference.Seconds;
                            double l = s.timespan_seconds - k;
                            flights.Add(f);
                            f.Latitude = (startLat * l + s.Latitude * k) / (l + k);
                            f.Longitude = (startLong * l + s.Longitude * k) / (l + k);
                            break;
                        }
                        else
                        {
                            // save the start location of the segment
                            startLat = s.Latitude;
                            startLong = s.Longitude;
                        }
                    }
                    f.Id = fp.Id;
                    //date time
                    f.Passengers = fp.Passengers;
                    f.Company_name = fp.Company_name;
                    // f.date_time
                    f.Is_external = false;
                }
                if (syncAll)
                {
                    List<Flight> externalFlights = null;
                    foreach (Server s in _context.Server)
                    {
                        string get = s.ServerURL + "api/Flights?relative_to=" + relative_to + "&syncAll=false";
                        externalFlights = GetFlightFromSever<List<Flight>>(get);
                        foreach (Flight f in externalFlights)
                        {
                            f.Is_external = true;
                        }
                    }
                    foreach (Flight f in externalFlights)
                    {
                        flights.Add(f);
                    }
                }
            }

            return flights;
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


        // GET: api/Flights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlight(string id)
        {
            var flight = await _context.Flight.FindAsync(id);
            if (flight == null)
            {
                foreach (Server s in _context.Server)
                {
                    string get = s.ServerURL + "api/Flights/" + id;
                    flight = GetFlightFromSever<Flight>(get);
                    if (flight == null)
                    {
                        return NotFound();
                    }
                    flight.Is_external = true;
                }
            }
            return flight;
        }

        // PUT: api/Flights/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlight(string id, Flight flight)
        {
            if (id != flight.Id)
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

        // POST: api/Flights
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Flight>> PostFlight(Flight flight)
        {
            _context.Flight.Add(flight);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFlight", new { id = flight.Id }, flight);
        }

        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Flight>> DeleteFlight(string id)
        {
            var flight = await _context.Flight.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            _context.Flight.Remove(flight);
            await _context.SaveChangesAsync();

            return flight;
        }

        private bool FlightExists(string id)
        {
            return _context.Flight.Any(e => e.Id.CompareTo(id) == 0);
        }
    }
}
