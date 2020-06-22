using CineGest.Data;
using CineGest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IEnumerable> GetCinemas()
        {
            return await _context.Cinema.Select(m => new { m.Id, m.Name, m.City, m.Location, m.Capacity }).ToListAsync();
        }

        // GET: api/cinemas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cinemas>> GetCinema(int id)
        {
            var cinema = await _context.Cinema.FindAsync(id);

            if (cinema == null)
            {
                return NotFound("Não existe nenhum cinema com esse ID.");
            }

            return cinema;
        }

        // PUT: api/Cinemas/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCinema(int id, [FromForm] string Name, [FromForm] string Location, [FromForm] string City, [FromForm] int Capacity)
        {
            try
            {
                var cinema = await _context.Cinema.FindAsync(id);
                cinema.Name = Name;
                cinema.Location = Location;
                cinema.Capacity = Capacity;
                cinema.City = City;

                _context.Entry(cinema).State = EntityState.Modified;


                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!CinemaExists(id))
                {
                    return NotFound("Não existe nenhum cinema com esse ID.");
                }
                else if (_context.Cinema.Where(c => c.Name == Name).FirstOrDefault() != null)
                {
                    return BadRequest("Já existe um cinema com esse nome.");
                }
                else
                {
                    return BadRequest("Erro inesperado.");
                }
            }

            return Ok();
        }

        // POST: api/Cinemas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Cinemas>> PostCinema([FromForm] string Name, [FromForm] string Location, [FromForm] string City, [FromForm] int Capacity)
        {
            try
            {
                Cinemas cinema = new Cinemas
                {
                    Name = Name,
                    Location = Location,
                    Capacity = Capacity,
                    City = City
                };

                _context.Cinema.Add(cinema);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("Já existe um cinema com esse nome.");
            }
            catch (Exception)
            {
                return BadRequest("Erro inesperado.");
            }
            return StatusCode(201);
        }

        // DELETE: api/Cinemas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Cinemas>> DeleteCinema(int id)
        {
            var cinemas = await _context.Cinema.FindAsync(id);
            if (cinemas == null)
            {
                return NotFound("Não existe nenhum cinema com esse ID.");
            }

            _context.Cinema.Remove(cinemas);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool CinemaExists(int id)
        {
            return _context.Cinema.Any(e => e.Id == id);
        }
    }
}
