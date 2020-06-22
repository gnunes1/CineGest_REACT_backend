using CineGest.Data;
using CineGest.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
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

        /// <summary>
        /// Defines the _environment.
        /// </summary>
        public static IWebHostEnvironment _environment;
        public UsersController(CineGestDB context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;

        }

        // GET: api/Users/others
        [HttpGet, Route("others")]
        public async Task<ActionResult<IEnumerable>> GetOthers()
        {
            return await _context.User.Where(u => u.Token != HttpContext.Request.Headers["token"].ToString())
                .Select(u => new { u.Id, u.Email, u.Name, Role = u.Role.Name, DoB = u.DoB.ToString("d", new CultureInfo("pt-PT")) }).ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(int id)
        {
            return Ok(await _context.User.Select(u => new { u.Id, u.Email, u.Name, Role = u.Role.Name, DoB = u.DoB.ToString("yyyy-MM-dd") })
            .Where(u => u.Id == id).FirstOrDefaultAsync());
        }

        // GET: api/Users/authenticated
        [HttpGet, Route("authenticated")]
        public async Task<ActionResult<Users>> GetAuthenticated()
        {

            //token do utilizador atual
            var token = Request.Headers["Token"].ToString();

            // pesquisar pelo token na base de dados
            var user = await _context.User.Where(u => u.Token == token).FirstOrDefaultAsync();

            // Call the next delegate/middleware in the pipeline
            if (user == null)
            {
                return NotFound("Não existe nenhum utilizador com este token");
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
        public async Task<IActionResult> PutUser(int id, [FromForm] DateTime DoB, [FromForm] string Email, [FromForm] string Name, [FromForm] string Password, [FromForm] string Role, IFormFile Avatar)
        {
            try
            {
                var user = await _context.User.FindAsync(id);

                user.DoB = DoB;
                user.Email = Email;
                user.Name = Name;
                user.TokenExpiresAt = new DateTime();
                user.Role = await _context.Roles.Where(r => r.Name == Role).FirstOrDefaultAsync();

                //password encriptada
                if (Password != null) user.Hash = BitConverter.ToString(SHA256.Create().ComputeHash(UTF8Encoding.Default.GetBytes(Password)));

                _context.Entry(user).State = EntityState.Modified;

                string[] oldName = user.Avatar.Split('/');

                if (Avatar == null)
                {
                    await _context.SaveChangesAsync();
                }

                else if (!Avatar.ContentType.Contains("image")) //se o poster não fôr imagem usa-se a imagem default
                {
                    user.Avatar = _environment.WebRootPath + "/images/users/default.png";

                    await _context.SaveChangesAsync();

                    if (oldName[^1] != "default.png") System.IO.File.Delete(_environment.WebRootPath + "/images/users/" + oldName[^1]);
                }
                else
                { //update atualiza a imagem
                    Guid g;
                    g = Guid.NewGuid();

                    string extensao = Path.GetExtension(Avatar.FileName).ToLower();

                    user.Avatar = Environment.GetEnvironmentVariable("ASPTNETCORE_APP_URL") + "images/users/" + g.ToString() + extensao;

                    await _context.SaveChangesAsync();

                    using var fileStream = new FileStream(_environment.WebRootPath + "/images/users/" + g.ToString() + extensao, FileMode.Create);
                    await Avatar.CopyToAsync(fileStream);

                    if (oldName[^1] != "default.png") System.IO.File.Delete(_environment.WebRootPath + "/images/users/" + oldName[^1]);

                }
            }
            catch (DbUpdateException)
            {
                if (!UserExists(id))
                {
                    return NotFound("Não existe nenhum filme com esse ID.");
                }
                else if (_context.Cinema.Where(c => c.Name == Name).FirstOrDefault() != null)
                {
                    return BadRequest("Já existe um filme com esse nome.");
                }
                else
                {
                    return BadRequest("Erro inesperado.");
                }
            }
            return StatusCode(201);

        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult> PostUser([FromForm] DateTime DoB, [FromForm] string Email, [FromForm] string Name, [FromForm] string Password, [FromForm] string Role, IFormFile Avatar)
        {

            //cria user
            try
            {
                Users user = new Users
                {
                    DoB = DoB,
                    Email = Email,
                    Name = Name,
                    TokenExpiresAt = new DateTime(),
                    Role = await _context.Roles.Where(r => r.Name == Role).FirstOrDefaultAsync(),

                    //password encriptada
                    Hash = BitConverter.ToString(SHA256.Create().ComputeHash(UTF8Encoding.Default.GetBytes(Password)))
                };

                //se o poster não fôr imagem usa-se a imagem default
                if (!Avatar.ContentType.Contains("image"))
                {
                    string path = _environment.WebRootPath + "/images/images/default.png";

                    user.Avatar = path;

                    _context.User.Add(user);
                    await _context.SaveChangesAsync();
                }
                else
                { //usa a imagem recebida
                    Guid g;
                    g = Guid.NewGuid();

                    string extensao = Path.GetExtension(Avatar.FileName).ToLower();

                    // caminho do ficheiro 
                    user.Avatar = Environment.GetEnvironmentVariable("ASPTNETCORE_APP_URL") + "images/users/" + g.ToString() + extensao;

                    _context.User.Add(user);
                    await _context.SaveChangesAsync();

                    using var fileStream = new FileStream(_environment.WebRootPath + "/images/users/" + g.ToString() + extensao, FileMode.Create);
                    await Avatar.CopyToAsync(fileStream);
                }


            }
            catch (DbUpdateException)
            {
                return BadRequest("Já existe um utilizador com este email.");
            }
            catch (Exception)
            {
                return BadRequest("Erro inesperado.");
            }
            return StatusCode(201);

        }

        // POST: api/Users/signup
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

            Users user = new Users
            {
                DoB = DoB,
                Email = Email,
                Name = Name,
                TokenExpiresAt = new DateTime(),

                // role default -> User
                Role = await _context.Roles.FindAsync(2),

                //password encriptada
                Hash = BitConverter.ToString(SHA256.Create().ComputeHash(UTF8Encoding.Default.GetBytes(Password)))
            };

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
            return StatusCode(201);

        }

        // POST: api/Users/login
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
                return NotFound("Email ou password incorretos.");
            }

            //cria da password do utilizador
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
        public async Task<ActionResult<Users>> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound("Não existe nenhum utilizador com esse ID.");
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            string[] oldName = user.Avatar.Split('/');
            if (oldName[^1] != "default.png") System.IO.File.Delete(_environment.WebRootPath + "/images/users/" + oldName[^1]);

            return Ok();
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
