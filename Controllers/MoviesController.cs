using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using F24_Assignment3_mwebster.Data;
using F24_Assignment3_mwebster.Models;
using System.Numerics;
using VaderSharp2;

namespace F24_Assignment3_mwebster.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }


            MovieDetailsVM movieDetailsVM = new MovieDetailsVM();
            movieDetailsVM.movie = movie;

            //get actors from db
            var actors = new List<Actor>();

            actors = await (from a in  _context.Actor
                            join am in _context.ActorMovie on a.Id equals am.ActorId
                            where am.MovieId == id
                            select a).ToListAsync();
            movieDetailsVM.actors = actors;

            var redditPosts = await RedditService.SearchRedditAsync(movie.Title);
            var redditAnalysis = await GetMovieSentimentAsync(movie.Title);

            movieDetailsVM.redditAnalysis = redditAnalysis;

            return View(movieDetailsVM);
        }
        public async Task<MovieRedditAnalysisVM> GetMovieSentimentAsync(string movieTitle)
        {
            // Call the RedditService to get Reddit posts related to the movie title
            var textToExamine = await RedditService.SearchRedditAsync(movieTitle);

            var redditAnalyzer = new SentimentIntensityAnalyzer();
            var redditSentiments = new List<RedditSentiment>();

            double totalSentiment = 0;
            int validCount = 0;

            foreach (var text in textToExamine)
            {
                var results = redditAnalyzer.PolarityScores(text);
                if (results.Compound != 0) // Ignore neutral sentiments
                {
                    redditSentiments.Add(new RedditSentiment
                    {
                        RedditText = text,
                        SentimentScore = results.Compound
                    });
                    totalSentiment += results.Compound;
                    validCount++;
                }
            }

            double overallSentiment = validCount > 0 ? totalSentiment / validCount : 0;

            return new MovieRedditAnalysisVM
            {
                MovieTitle = movieTitle,
                RedditSentiments = redditSentiments,
                OverallSentiment = overallSentiment
            };
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> GetMoviePhoto(int id)
        {
            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if(movie == null)
            {
                return NotFound();
            }
            var imageData = movie.MovieImage;
            return File(imageData, "image/jpg");
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Imdb,Genre,ReleaseDate")] Movie movie, IFormFile MovieImage)
        {
            ModelState.Remove(nameof(movie.MovieImage));

            if (ModelState.IsValid)
            {
                if (MovieImage != null && MovieImage.Length > 0)
                {
                    var memoryStream = new MemoryStream();
                    await MovieImage.CopyToAsync(memoryStream);
                    movie.MovieImage = memoryStream.ToArray();
                }
                else
                {
                    movie.MovieImage = new byte[0];
                }
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Imdb,Genre,ReleaseDate")] Movie movie, IFormFile MovieImage)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(movie.MovieImage));

            if (ModelState.IsValid)
            {
                try
                {
                    // If a new image was uploaded, replace the existing one
                    if (MovieImage != null && MovieImage.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await MovieImage.CopyToAsync(memoryStream);
                            movie.MovieImage = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        // Retain the existing image if no new image was uploaded
                        var existingMovie = await _context.Movie.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                        if (existingMovie != null)
                        {
                            movie.MovieImage = existingMovie.MovieImage;
                        }
                    }

                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(movie);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!MovieExists(movie.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(movie);
        //}

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
