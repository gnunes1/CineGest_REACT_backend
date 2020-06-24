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
    public class SessionsController : ControllerBase
    {
        private readonly CineGestDB _context;

        public SessionsController(CineGestDB context)
        {
            _context = context;
        }

        // POST: api/Sessions/movie
        [HttpPost, Route("movie")]
        public async Task<ActionResult<IEnumerable>> GetSessionsByMovie([FromForm] int Movie)
        {
            return await _context.Sessions.Where(s => s.Movie.Id == Movie).ToListAsync();
        }

        // GET: api/Sessions
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetSessions()
        {
            return await _context.Sessions.OrderBy(s => s.Cinema).ThenBy(s => s.Start).Select(s => new
            {
                s.Id,
                Start = s.Start.ToString("yyyy-MM-dd HH:mm"),
                End = s.End.ToString("yyyy-MM-dd HH:mm"),
                Movie = s.Movie.Name,
                Cinema = s.Cinema.Name
            }).ToListAsync();
        }


        /// GET: api/Sessions/highlighted
        [HttpGet, Route("highlighted")]
        public async Task<IEnumerable> GetHighlightedMovies()
        {
            return await _context.Sessions.Where(s => s.Movie.Highlighted == true)
                .Select(s => new
                {
                    s.Movie.Id,
                    Min = _context.Sessions.Where(ss => ss.Movie.Id == s.Movie.Id).Min(s => s.Start).ToString("dd-MM-yyyy"),
                    Max = _context.Sessions.Where(ss => ss.Movie.Id == s.Movie.Id).Max(s => s.End).ToString("dd-MM-yyyy")
                })
                .Distinct()
                .ToListAsync();

        }

        // PUT: api/Sessions/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSession(int id, [FromForm] DateTime Start, [FromForm] DateTime End, [FromForm] int Cinema, [FromForm] int Movie)
        {
            try
            {
                Start = Start.ToUniversalTime();
                End = End.ToUniversalTime();

                //verifica se já existe uma cinema a decorrer
                if (await _context.Sessions.Where(s => Start >= s.Start && Start <= s.End && s.Cinema.Id == Cinema).AnyAsync())
                    return BadRequest("Já existe uma sessão neste cinema entre esta data.");

                Movies movie = await _context.Movie.Where(c => c.Id == Movie).FirstOrDefaultAsync();

                var session = await _context.Sessions.FindAsync(id);

                session.Start = Start;
                session.End = Start.AddMinutes(movie.Duration);
                session.Cinema = await _context.Cinema.Where(c => c.Id == Cinema).FirstOrDefaultAsync();
                session.Movie = movie;


                _context.Entry(session).State = EntityState.Modified;


                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!SessionExists(id))
                {
                    return NotFound("Nenhuma sessão com este ID foi encontrada.");
                }
                else
                {
                    return BadRequest("Erro inesperado");
                }
            }

            return Ok();
        }

        // PUT: api/Sessions/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSessionSeats(int id)
        {
            try
            {
                var session = await _context.Sessions.FindAsync(id);

                session.Occupated_seats++;

                _context.Entry(session).State = EntityState.Modified;


                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!SessionExists(id))
                {
                    return NotFound("Nenhuma sessão com este ID foi encontrada.");
                }
                else
                {
                    return BadRequest("Erro inesperado");
                }
            }

            return Ok();
        }

        // POST: api/Sessions
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Sessions>> PostSession([FromForm] DateTime Start, [FromForm] DateTime End, [FromForm] int Cinema, [FromForm] int Movie)
        {

            Start = Start.ToUniversalTime();
            End = End.ToUniversalTime();

            //verifica se já existe uma cinema a decorrer
            if (await _context.Sessions.Where(s => Start >= s.Start && Start <= s.End && s.Cinema.Id == Cinema).AnyAsync())
                return BadRequest("Já existe uma sessão neste cinema entre esta data.");

            Sessions session = new Sessions()
            {
                Start = Start,
                End = End,
                Cinema = await _context.Cinema.Where(c => c.Id == Cinema).FirstOrDefaultAsync(),
                Movie = await _context.Movie.Where(c => c.Id == Movie).FirstOrDefaultAsync(),
                Occupated_seats = 0
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        // DELETE: api/Sessions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Sessions>> DeleteSession(int id)
        {

            var sessions = await _context.Sessions.FindAsync(id);
            if (sessions == null)
            {
                return NotFound("Não existe nenhuma sessão com este ID.");
            }

            _context.Sessions.Remove(sessions);
            await _context.SaveChangesAsync();

            return Ok();

        }

        private bool SessionExists(int id)
        {
            return _context.Sessions.Any(e => e.Id == id);
        }
    }
}
