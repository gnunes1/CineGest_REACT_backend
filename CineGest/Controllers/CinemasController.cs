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
    public class CinemasController : ControllerBase
    {
        private readonly CineGestDB _context;

        public CinemasController(CineGestDB context)
        {
            _context = context;
        }

        // GET: api/Cinemas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cinemas>>> GetCinema()
        {
            return await _context.Cinema.ToListAsync();
        }

        // GET: api/Cinemas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cinemas>> GetCinemas(int id)
        {
            var cinemas = await _context.Cinema.FindAsync(id);

            if (cinemas == null)
            {
                return NotFound();
            }

            return cinemas;
        }

        // PUT: api/Cinemas/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCinemas(int id, Cinemas cinemas)
        {
            if (id != cinemas.Id)
            {
                return BadRequest();
            }

            _context.Entry(cinemas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CinemasExists(id))
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

        // POST: api/Cinemas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Cinemas>> PostCinemas(Cinemas cinemas)
        {
            _context.Cinema.Add(cinemas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCinemas", new { id = cinemas.Id }, cinemas);
        }

        // DELETE: api/Cinemas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Cinemas>> DeleteCinemas(int id)
        {
            var cinemas = await _context.Cinema.FindAsync(id);
            if (cinemas == null)
            {
                return NotFound();
            }

            _context.Cinema.Remove(cinemas);
            await _context.SaveChangesAsync();

            return cinemas;
        }

        private bool CinemasExists(int id)
        {
            return _context.Cinema.Any(e => e.Id == id);
        }
    }
}
