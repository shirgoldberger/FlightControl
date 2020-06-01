using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Data;
using FlightControlWeb.Models;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private readonly FlightDbContext context;

        public ServersController(FlightDbContext c)
        {
            context = c;
        }

        // GET: api/servers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Server>>> GetServer()
        {
            return await context.Server.ToListAsync();
        }

        // GET: api/servers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Server>> GetServer(string id)
        {
            var server = await context.Server.FindAsync(id);

            if (server == null)
            {
                return NotFound();
            }

            return server;
        }

        // PUT: api/servers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServer(string id, Server server)
        {
            if (id.CompareTo(server.ServerID) != 0)
            {
                return BadRequest();
            }

            context.Entry(server).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServerExists(id))
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

        // POST: api/Servers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Server>> PostServer(Server server)
        {
            server.ServerID = IDGenerator();
            context.Server.Add(server);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ServerExists(server.ServerID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetServer", new { id = server.ServerID }, server);
        }

        public string IDGenerator()
        {
            // Create random ID that look like- '000-000-000'.
            string id = "";
            int num1 = LocalLibrary.GetNumber();
            id = id + num1;
            int num2 = LocalLibrary.GetNumber();
            id = id + num2;
            int num3 = LocalLibrary.GetNumber();
            id = id + num3;
            id = id + "-";
            int num4 = LocalLibrary.GetNumber();
            id = id + num4;
            int num5 = LocalLibrary.GetNumber();
            id = id + num5;
            int num6 = LocalLibrary.GetNumber();
            id = id + num6;
            id = id + "-";
            int num7 = LocalLibrary.GetNumber();
            id = id + num7;
            int num8 = LocalLibrary.GetNumber();
            id = id + num8;
            int num9 = LocalLibrary.GetNumber();
            id = id + num9;
            return id;
        }

        // DELETE: api/Servers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Server>> DeleteServer(string id)
        {
            var server = await context.Server.FindAsync(id);
            if (server == null)
            {
                return NotFound();
            }

            context.Server.Remove(server);
            await context.SaveChangesAsync();

            return server;
        }

        private bool ServerExists(string id)
        {
            return context.Server.Any(e => id.CompareTo(e.ServerID) == 0);
        }
    }
}