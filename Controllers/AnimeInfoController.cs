using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnimeAPI.Models;
using JikanDotNet;

namespace AnimeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimeInfoController : ControllerBase
    {
        private readonly AnimeInfoContext _context;

        public AnimeInfoController(AnimeInfoContext context)
        {
            _context = context;
        }

        // GET: api/AnimeInfos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnimeInfo>>> GetAnimeInfos()
        {
            return await _context.AnimeInfos.ToListAsync();
        }

        // GET: api/AnimeInfos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AnimeInfo>> GetAnimeInfos(string id)
        {
            IJikan jikan = new Jikan();
            
            AnimeInfo animeInfo = new AnimeInfo();
            AnimeSearchResult animes = await jikan.SearchAnime(id);            
            if (animes.Results.Count <= 0)
            {
                animeInfo.Title = "nsrf";                
            } else 
            {
                AnimeSearchEntry firstFound = animes.Results.First();
                animeInfo.Title = firstFound.Title;
                animeInfo.ReleaseDate = firstFound.StartDate.Value.ToShortDateString();
                animeInfo.NumberOfEpisodes = firstFound.Episodes.GetValueOrDefault();
                animeInfo.Summary = firstFound.Description;                
            }

            return animeInfo;
        }

        // PUT: api/AnimeInfos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnimeInfos(int id, AnimeInfo AnimeInfos)
        {
            if (id != AnimeInfos.id)
            {
                return BadRequest();
            }

            _context.Entry(AnimeInfos).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimeInfosExists(id))
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

        // POST: api/AnimeInfos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AnimeInfo>> PostAnimeInfos(AnimeInfo AnimeInfos)
        {
            _context.AnimeInfos.Add(AnimeInfos);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAnimeInfos", new { id = AnimeInfos.id }, AnimeInfos);
        }

        // DELETE: api/AnimeInfos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimeInfos(int id)
        {
            var AnimeInfos = await _context.AnimeInfos.FindAsync(id);
            if (AnimeInfos == null)
            {
                return NotFound();
            }

            _context.AnimeInfos.Remove(AnimeInfos);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AnimeInfosExists(int id)
        {
            return _context.AnimeInfos.Any(e => e.id == id);
        }
    }
}
