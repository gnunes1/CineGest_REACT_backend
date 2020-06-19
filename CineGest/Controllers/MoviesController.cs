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

namespace CineGest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        public static IWebHostEnvironment _environment;

        private readonly CineGestDB _context;

        public MoviesController(CineGestDB context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/Movies
        //retorna os todos os filmes com informação limitada
        [HttpGet]
        public async Task<IEnumerable> GetMovies()
        {
            var movies = await _context.Movie.Select(m => new { m.Id, m.Name, m.Poster }).ToListAsync();

            List<(int, string, FileResult)> movies2 = new List<(int, string, FileResult)>();


            foreach (var item in movies)
            {
                var myfile = System.IO.File.ReadAllBytes(_environment.WebRootPath + "/images/movies/" + item.Poster + ".jpg");
                movies2.Add((item.Id, item.Name, new FileContentResult(myfile, "application/pdf")));
            }

            return movies2;
        }

        // GET: api/Movies 
        //retorna todos os filmes com toda a informação
        [HttpGet, Route("moviesdetails")]
        public async Task<ActionResult<IEnumerable<Movies>>> GetMovieDetails()
        {
            return await _context.Movie.ToListAsync();
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movies>> GetMovie(int id)
        {
            var movies = await _context.Movie.FindAsync(id);

            if (movies == null)
            {
                return NotFound();
            }

            return movies;
        }

        // PUT: api/Movies/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovies(int id, Movies movies)
        {
            if (id != movies.Id)
            {
                return BadRequest();
            }

            _context.Entry(movies).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MoviesExists(id))
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

        // POST: api/Movies
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult> PostMovies([FromForm][Bind("Name, Description, Min_age, Genres, Duration, Highlighted")] Movies Movie, [FromForm] IFormFile Poster)
        {
            try
            {
                Guid g;
                g = Guid.NewGuid();

                string extensao = Path.GetExtension(Poster.FileName).ToLower();

                // nome do ficheiro 
                string nome = g.ToString();

                using var fileStream = new FileStream(_environment.WebRootPath + "/images/movies/" + nome + extensao, FileMode.Create);
                await Poster.CopyToAsync(fileStream);

                Movie.Poster = nome;
                _context.Movie.Add(Movie);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Um filme com o mesmo nome já foi inserido");
            }
            catch (Exception)
            {
                return BadRequest("Erro inesperado.");
            }
            return Ok("Filme criado.");
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Movies>> DeleteMovies(int id)
        {
            var movies = await _context.Movie.FindAsync(id);
            if (movies == null)
            {
                return NotFound();
            }

            _context.Movie.Remove(movies);
            await _context.SaveChangesAsync();

            return movies;
        }

        private bool MoviesExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
