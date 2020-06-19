using CineGest.Data;
using CineGest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

        // GET: api/Users/current
        [HttpGet, Route("authenticated")]
        public async Task<ActionResult<Users>> GetUsers()
        {

            //token do utilizador atual
            var token = Request.Headers["Token"].ToString();

            // pesquisar pelo token na base de dados
            var user = await _context.User.Where(u => u.Token == token).FirstOrDefaultAsync();

            // Call the next delegate/middleware in the pipeline
            if (user == null)
            {
                return NotFound();
            }

            var userRole = _context.Roles.Where(r => r.Id == user.RoleFK).Select(r => r.Name).First();

            return Ok(new
            {
                name = user.Name,
                role = userRole
            });
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
        public async Task<ActionResult> Signup([FromForm] DateTime DoB, [FromForm] string Email, [FromForm] string Name, [FromForm] string Password)
        {

            Users user = new Users();
            user.DoB = DoB;
            user.Email = Email;
            user.Name = Name;
            user.TokenExpiresAt = new DateTime();

            // role default -> User
            user.Role = await _context.Roles.FindAsync(2);

            //password encriptada
            user.Hash = BitConverter.ToString(SHA256.Create().ComputeHash(UTF8Encoding.Default.GetBytes(Password)));

            //cria user
            try
            {
                _context.User.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("Já existe um utilizador com este email.");
            }
            catch (Exception)
            {
                return BadRequest();
            }
            return StatusCode(201, "Utilizador criado.");

        }
        /// <summary>
        /// Login do utilizador
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost, Route("login")]
        public async Task<ActionResult> Login([FromForm] string Email, [FromForm] string Password)
        {
            //verifica se o email existe
            var dbUser = await _context.User.Where(u => u.Email == Email).FirstOrDefaultAsync();

            if (dbUser == null)
            {
                return BadRequest("Email ou password incorretos.");
            }

            //cria token para o utilizador
            String hash = BitConverter.ToString(SHA256.Create().ComputeHash(UTF8Encoding.Default.GetBytes(Password)));

            if (hash != dbUser.Hash)
            {
                return BadRequest("Email ou password incorretos.");
            }

            //role do user
            var dbUserRole = _context.Roles.Where(r => r.Id == dbUser.RoleFK).Select(r => r.Name).First();

            //token
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            //update do token e da data de criação do token na base de dados referente ao dbUser
            dbUser.Token = token;
            dbUser.TokenExpiresAt = DateTime.UtcNow.AddDays(2);

            _context.SaveChanges();

            return Ok(new
            {
                token,
                role = dbUserRole,
                name = dbUser.Name

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
