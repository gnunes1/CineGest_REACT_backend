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
    public class TicketsController : ControllerBase
    {
        private readonly CineGestDB _context;

        public TicketsController(CineGestDB context)
        {
            _context = context;
        }

        // GET: api/Tickets
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetTickets()
        {
            return await _context.Ticket.Select(t => new
            {
                t.Id,
                Movie = t.Cinema_Movie.Movie.Name,
                City = t.Cinema_Movie.Cinema.City,
                Location = t.Cinema_Movie.Cinema.Location,
                t.Cinema_Movie.Start,
                t.Cinema_Movie.End,
                Cinema = t.Cinema_Movie.Cinema.Name,
                t.User.Email,
                t.Seat
            }).OrderBy(t => t.Start).ThenBy(t => t.Email).ToListAsync();
        }

        // GET: api/Tickets
        [HttpGet, Route("current")]
        public async Task<ActionResult<IEnumerable>> GetTicketsCurrent()
        {
            return await _context.Ticket.Where(t => t.User.Token == HttpContext.Request.Headers["token"].ToString())
                .Select(t => new
                {
                    t.Id,
                    t.Seat,
                    Cinema = t.Cinema_Movie.Cinema.Name,
                    City = t.Cinema_Movie.Cinema.City,
                    Location = t.Cinema_Movie.Cinema.Location,
                    t.Cinema_Movie.Start,
                    t.Cinema_Movie.End,
                    Movie = t.Cinema_Movie.Movie.Name
                }).OrderBy(s => s.Start).ThenBy(s => s.Cinema).ThenBy(s => s.Movie)
                .ToListAsync();
        }

        // POST: api/Tickets
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Tickets>> BuyTicket([FromForm] int Session)
        {
            if (Session == null) return BadRequest("Nenhuma sessão selecionada");

            if (!await _context.Sessions.AnyAsync(e => e.Id == Session)) return NotFound("Não existe nenhuma sessão com este ID.");

            var qtt = await _context.Ticket.Where(t => t.Cinema_Movie == _context.Sessions.Find(Session)).CountAsync();

            if (qtt >= await _context.Sessions.Where(s => s.Id == Session).Select(s => s.Cinema.Capacity).FirstOrDefaultAsync())
            {
                return BadRequest("Sessão esgotada.");
            }
            if (await _context.Ticket.Where(t => t.Cinema_Movie.Id == Session && t.User.Token == HttpContext.Request.Headers["token"].ToString()).AnyAsync())
            {
                return BadRequest("Já adquiriu este bilhete préviamente.");
            }

            var userDob = await _context.User.Where(u => u.Token == HttpContext.Request.Headers["token"].ToString()).Select(t => t.DoB).FirstOrDefaultAsync();
            var movieAgeReq = await _context.Sessions.Select(s => s.Movie.Min_age).FirstOrDefaultAsync();


            if (DateTime.UtcNow.Subtract(userDob).Ticks - DateTime.UtcNow.AddYears(-18).Ticks > 0)
            {
                return BadRequest("Não tem idade para assistir a este filme.");
            }

            Tickets ticket = new Tickets
            {
                Cinema_Movie = await _context.Sessions.FindAsync(Session),
                Seat = qtt + 1,
                User = await _context.User.Where(u => u.Token == HttpContext.Request.Headers["token"].ToString()).FirstOrDefaultAsync(),
            };

            var session = await _context.Sessions.Where(s => s.Id == Session).FirstOrDefaultAsync();
            session.Occupated_seats += 1;

            _context.Ticket.Add(ticket);
            await _context.SaveChangesAsync();

            return StatusCode(201);

        }

        // DELETE: api/Tickets/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Tickets>> DeleteTicket(int id)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket == null)
            {
                return NotFound("Nenhum ticket encontrado com esse ID.");
            }

            _context.Ticket.Remove(ticket);
            await _context.SaveChangesAsync();

            return ticket;
        }

        private bool TicketExists(int id)
        {
            return _context.Ticket.Any(e => e.Id == id);
        }
    }
}
