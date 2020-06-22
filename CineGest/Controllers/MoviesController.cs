namespace CineGest.Controllers
{
    using CineGest.Data;
    using CineGest.Models;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="MoviesController" />.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        /// <summary>
        /// Defines the _environment.
        /// </summary>
        public static IWebHostEnvironment _environment;

        /// <summary>
        /// Defines the _context.
        /// </summary>
        private readonly CineGestDB _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoviesController"/> class.
        /// </summary>
        /// <param name="context">The context<see cref="CineGestDB"/>.</param>
        /// <param name="environment">The environment<see cref="IWebHostEnvironment"/>.</param>
        public MoviesController(CineGestDB context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/Movies
        //retorna os todos os filmes com informação limitada
        /// <summary>
        /// The GetMovies.
        /// </summary>
        /// <returns>The <see cref="Task{IEnumerable}"/>.</returns>
        [HttpGet]
        public async Task<IEnumerable> GetMovies()
        {
            var movies = await _context.Movie.Select(m => new { m.Id, m.Name, m.Poster }).ToListAsync();
            return movies;
        }
        // GET: api/Movies
        //retorna os todos os filmes com informação limitada
        /// <summary>
        /// The GetMovies.
        /// </summary>
        /// <returns>The <see cref="Task{IEnumerable}"/>.</returns>
        [HttpGet, Route("highlighted")]
        public async Task<IEnumerable> GetHighlightedMovies()
        {
            var movies = await _context.Movie.Where(m => m.Highlighted == true)
                //.Join(_context.Sessions)
                .Select(m => new { m.Id, m.Name, m.Poster })
                .ToListAsync();
            return movies;
        }

        // GET: api/Movies 
        //retorna todos os filmes com toda a informação
        /// <summary>
        /// The GetMovieDetails.
        /// </summary>
        /// <returns>The <see cref="Task{ActionResult{IEnumerable{Movies}}}"/>.</returns>
        [HttpGet, Route("details")]
        public async Task<ActionResult<IEnumerable<Movies>>> GetMoviesDetails()
        {
            return await _context.Movie.ToListAsync();
        }

        // GET: api/Movies/5
        /// <summary>
        /// The GetMovie.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult{Movies}}"/>.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Movies>> GetMovie(int id)
        {
            var movie = await _context.Movie.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return await _context.Movie.Select(m => new Movies { Id = m.Id, Highlighted = m.Highlighted, Min_age = m.Min_age, Name = m.Name, Genres = m.Genres, Description = m.Description, Poster = m.Poster, Duration = m.Duration }).Where(m => m.Id == id).FirstOrDefaultAsync();
        }

        // PUT: api/Movies/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// The PutMovies.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="movies">The movies<see cref="Movies"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, [FromForm] string Name, [FromForm] string Description, [FromForm] int Min_age,
            [FromForm] string Genres, [FromForm] int Duration, [FromForm] bool Highlighted, [FromForm] IFormFile Poster)
        {
            try
            {
                var movie = await _context.Movie.FindAsync(id);
                movie.Name = Name;
                movie.Description = Description;
                movie.Min_age = Min_age;
                movie.Genres = Genres;
                movie.Duration = Duration;
                movie.Highlighted = Highlighted;

                _context.Entry(movie).State = EntityState.Modified;

                string[] oldName = movie.Poster.Split('/');


                if (Poster == null)
                {
                    await _context.SaveChangesAsync();
                }

                else if (!Poster.ContentType.Contains("image")) //se o poster não fôr imagem usa-se a imagem default
                {
                    movie.Poster = _environment.WebRootPath + "/images/movies/default.png";

                    await _context.SaveChangesAsync();

                    if (oldName[^1] != "default.png") System.IO.File.Delete(_environment.WebRootPath + "/images/movies/" + oldName[^1]);
                }
                else
                { //update atualiza a imagem
                    Guid g;
                    g = Guid.NewGuid();

                    string extensao = Path.GetExtension(Poster.FileName).ToLower();

                    movie.Poster = Environment.GetEnvironmentVariable("ASPTNETCORE_APP_URL") + "images/movies/" + g.ToString() + extensao;

                    using var fileStream = new FileStream(_environment.WebRootPath + "/images/movies/" + g.ToString() + extensao, FileMode.Create);
                    await Poster.CopyToAsync(fileStream);

                    await _context.SaveChangesAsync();

                    if (oldName[^1] != "default.png") System.IO.File.Delete(_environment.WebRootPath + "/images/movies/" + oldName[^1]);

                }
            }
            catch (DbUpdateException)
            {
                if (!MovieExists(id))
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

            return Ok();
        }

        // POST: api/Movies
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// The PostMovies.
        /// </summary>
        /// <param name="Movie">The Movie<see cref="Movies"/>.</param>
        /// <param name="Poster">The Poster<see cref="IFormFile"/>.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpPost]
        public async Task<ActionResult> PostMovie([FromForm] string Name, [FromForm] string Description, [FromForm] int Min_age,
            [FromForm] string Genres, [FromForm] int Duration, [FromForm] bool Highlighted, [FromForm] IFormFile Poster)
        {
            try
            {
                Movies movie = new Movies
                {
                    Name = Name,
                    Description = Description,
                    Min_age = Min_age,
                    Genres = Genres,
                    Duration = Duration,
                    Highlighted = Highlighted
                };

                //se o poster não fôr imagem usa-se a imagem default
                if (!Poster.ContentType.Contains("image"))
                {
                    string path = _environment.WebRootPath + "/images/movies/default.png";

                    movie.Poster = path;

                    _context.Movie.Add(movie);
                    await _context.SaveChangesAsync();
                }
                else
                { //usa a imagem recebida
                    Guid g;
                    g = Guid.NewGuid();

                    string extensao = Path.GetExtension(Poster.FileName).ToLower();

                    // caminho do ficheiro 
                    movie.Poster = Environment.GetEnvironmentVariable("ASPTNETCORE_APP_URL") + "images/movies/" + g.ToString() + extensao;

                    _context.Movie.Add(movie);
                    await _context.SaveChangesAsync();

                    using var fileStream = new FileStream(_environment.WebRootPath + "/images/movies/" + g.ToString() + extensao, FileMode.Create);
                    await Poster.CopyToAsync(fileStream);
                }

            }
            catch (DbUpdateException)
            {
                return BadRequest("Um filme com o mesmo nome já foi inserido");
            }
            catch (Exception)
            {
                return BadRequest("Erro inesperado.");
            }
            return StatusCode(201);
        }

        // DELETE: api/Movies/5
        /// <summary>
        /// The DeleteMovies.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ActionResult{Movies}}"/>.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Movies>> DeleteMovie(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound("Não existe nenhum cinema com esse ID.");
            }

            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();

            string[] oldName = movie.Poster.Split('/');
            if (oldName[^1] != "default.png") System.IO.File.Delete(_environment.WebRootPath + "/images/movies/" + oldName[^1]);

            return Ok();
        }

        /// <summary>
        /// The MoviesExists.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
