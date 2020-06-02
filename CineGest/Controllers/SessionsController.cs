using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CineGest.Data;
using CineGest.Models;

namespace CineGest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly CineGestDB _context;

        public SessionsController(CineGestDB context)
        {
            _context = context;
        }

        // GET: api/Sessions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sessions>>> GetCinema_Movie()
        {
            return await _context.Cinema_Movie.ToListAsync();
        }

        // GET: api/Sessions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sessions>> GetSessions(int id)
        {
            var sessions = await _context.Cinema_Movie.FindAsync(id);

            if (sessions == null)
            {
                return NotFound();
            }

            return sessions;
        }

        // PUT: api/Sessions/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSessions(int id, Sessions sessions)
        {
            if (id != sessions.Id)
            {
                return BadRequest();
            }

            _context.Entry(sessions).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SessionsExists(id))
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

        // POST: api/Sessions
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Sessions>> PostSessions(Sessions sessions)
        {
            _context.Cinema_Movie.Add(sessions);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSessions", new { id = sessions.Id }, sessions);
        }

        // DELETE: api/Sessions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Sessions>> DeleteSessions(int id)
        {
            var sessions = await _context.Cinema_Movie.FindAsync(id);
            if (sessions == null)
            {
                return NotFound();
            }

            _context.Cinema_Movie.Remove(sessions);
            await _context.SaveChangesAsync();

            return sessions;
        }

        private bool SessionsExists(int id)
        {
            return _context.Cinema_Movie.Any(e => e.Id == id);
        }
    }
}
