using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using F24_Assignment3_mwebster.Data;
using F24_Assignment3_mwebster.Models;
using VaderSharp2;

namespace F24_Assignment3_mwebster.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Actors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Actor.ToListAsync());
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            ActorDetailsVM actorDetailsVM = new ActorDetailsVM();
            actorDetailsVM.actor = actor;

            //get movies from db
            var movies = new List<Movie>();

            movies = await (from m in _context.Movie
                            join am in _context.ActorMovie on m.Id equals am.MovieId
                            where am.ActorId == id
                            select m).ToListAsync();
            actorDetailsVM.movies = movies;

            var redditPosts = await RedditService.SearchRedditAsync(actor.Name);
            var redditAnalysis = await GetActorSentimentAsync(actor.Name);
            actorDetailsVM.redditAnalysis = redditAnalysis;

            return View(actorDetailsVM);
        }
        public async Task<ActorRedditAnalysisVM> GetActorSentimentAsync(string actorName)
        {
            // Call RedditService to fetch Reddit posts related to the actor's name
            var textToExamine = await RedditService.SearchRedditAsync(actorName);

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

            return new ActorRedditAnalysisVM
            {
                ActorName = actorName,
                RedditSentiments = redditSentiments,
                OverallSentiment = overallSentiment
            };
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> GetActorPhoto(int id)
        {
            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }
            var imageData = actor.ActorImage;
            return File(imageData, "image/jpg");
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Gender,Age,Imdb")] Actor actor, IFormFile ActorImage)
        {
            ModelState.Remove(nameof(actor.ActorImage));

            if (ModelState.IsValid)
            {
                if(ActorImage != null && ActorImage.Length > 0)
                {
                    var memoryStream = new MemoryStream();
                    await ActorImage.CopyToAsync(memoryStream);
                    actor.ActorImage = memoryStream.ToArray();
                }
                else
                {
                    actor.ActorImage = new byte[0];
                }
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Age,Imdb")] Actor actor, IFormFile ActorImage)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(actor.ActorImage));

            if (ModelState.IsValid)
            {
                try
                {
                    // If a new image was uploaded, replace the existing one
                    if (ActorImage != null && ActorImage.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await ActorImage.CopyToAsync(memoryStream);
                            actor.ActorImage = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        // Retain the existing image if no new image was uploaded
                        var existingActor = await _context.Actor.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
                        if (existingActor != null)
                        {
                            actor.ActorImage = existingActor.ActorImage;
                        }
                    }

                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
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
            return View(actor);
        }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(actor);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ActorExists(actor.Id))
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
        //    return View(actor);
        //}

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actor.FindAsync(id);
            if (actor != null)
            {
                _context.Actor.Remove(actor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actor.Any(e => e.Id == id);
        }
    }
}
