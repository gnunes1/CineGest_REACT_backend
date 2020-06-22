using CineGest.Data;
using CineGest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                t.Cinema_Movie.Start,
                t.Cinema_Movie.End,
                Cinema = t.Cinema_Movie.Cinema.Name,
                t.User.Email
            }).ToListAsync();
        }

        // GET: api/Tickets
        [HttpGet, Route("current")]
        public async Task<ActionResult<IEnumerable>> GetTicketsCurrent()
        {
            return await _context.Ticket.Where(t => t.User.Token == HttpContext.Request.Headers["token"])
                .Select(t => new
                {
                    t.Id,
                    t.User.Email,
                    t.Seat,
                    Cinema = t.Cinema_Movie.Cinema.Name,
                    t.Cinema_Movie.Start,
                    t.Cinema_Movie.End,
                    Movie = t.Cinema_Movie.Movie.Name
                })
                .ToListAsync();
        }

        // GET: api/Tickets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tickets>> GetTicket(int id)
        {
            var tickets = await _context.Ticket.FindAsync(id);

            if (tickets == null)
            {
                return NotFound("Nenhum ticket encontrado com esse ID.");
            }

            return tickets;
        }

        // PUT: api/Tickets/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTickets(int id, Tickets ticket)
        {

            _context.Entry(ticket).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(id))
                {
                    return NotFound("Nenhum ticket encontrado com esse ID.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Tickets
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Tickets>> PostTickets(Tickets tickets)
        {
            _context.Ticket.Add(tickets);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTickets", new { id = tickets.Id }, tickets);
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
