using CineGest.Data;
using CineGest.Models;
using CineGest.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CineGest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CineGestDB _context;

        public UsersController(CineGestDB context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetUsers(int id)
        {
            var users = await _context.User.FindAsync(id);

            if (users == null)
            {
                return NotFound();
            }

            return users;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsers(int id, Users users)
        {
            if (id != users.Id)
            {
                return BadRequest();
            }

            _context.Entry(users).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Registar utilizador
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost, Route("signup")]
        public async Task<ActionResult> Signup(Users user)
        {
            user.Role = await _context.Roles.FindAsync(2);

            //hash gerada apartir da password
            var hasher = new PasswordHasher<Users>();
            user.Password = hasher.HashPassword(user, user.Password);

            //cria user
            try
            {
                _context.User.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Já existe um utilizador com este email." });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Erro inesperado." });
            }

            return CreatedAtAction("GetUsers", new { id = user.Id }, user.Id);
        }
        /// <summary>
        /// Login do utilizador
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost, Route("login")]
        public async Task<ActionResult> Login(Users user)
        {
            var dbUser = await _context.User.Where(u => u.Email == user.Email).FirstOrDefaultAsync();

            if (dbUser == null)
            {
                return BadRequest();
            }

            //hash gerada apartir da password
            var hasher = new PasswordHasher<Users>();

            if (hasher.VerifyHashedPassword(dbUser, dbUser.Password, user.Password) == PasswordVerificationResult.Failed)
            {
                return BadRequest();
            }

            var dbUserRole = _context.Roles.Where(r => r.Id == dbUser.RoleFK).Select(r => r.Name).First();

            Token tk = new Token(dbUser.Id.ToString(), dbUserRole.ToString(), dbUser.Name);

            return Ok(new
            {
                Token = tk.getToken()
            });

        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Users>> DeleteUsers(int id)
        {
            var users = await _context.User.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            _context.User.Remove(users);
            await _context.SaveChangesAsync();

            return users;
        }

        private bool UsersExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
