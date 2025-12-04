using BrainBox.Data;
using BrainBox.Models;
using BrainBox.Models.DTOs.IdeaDto;
using BrainBox.Models.DTOs.ThemeDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrainBox.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdeasController : ControllerBase
    {
        private readonly BrainBoxDbContext _context;

        public IdeasController(BrainBoxDbContext context)
        {
            _context = context;
        }

        // GET: api/ideas
        // GET: api/ideas?createdFrom=2025-12-01&createdTo=2025-12-04&themeIds=1,3
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IdeaDto>>> GetIdeas(
            [FromQuery] DateTime? createdFrom = null,
            [FromQuery] DateTime? createdTo = null,
            [FromQuery] DateTime? modifiedFrom = null,
            [FromQuery] DateTime? modifiedTo = null,
            [FromQuery] string? themeIds = null)
        {
            var query = _context.Ideas.AsNoTracking();

            // Filtro data creazione
            if (createdFrom.HasValue)
            {
                query = query.Where(i => i.CreatedAt >= createdFrom.Value);
            }
            if (createdTo.HasValue)
            {
                var endOfDay = createdTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(i => i.CreatedAt <= endOfDay);
            }

            // Filtro data ultima modifica
            if (modifiedFrom.HasValue)
            {
                query = query.Where(i => i.LastModifiedAt >= modifiedFrom.Value);
            }
            if (modifiedTo.HasValue)
            {
                var endOfDay = modifiedTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(i => i.LastModifiedAt <= endOfDay);
            }

            // Filtro per temi
            if (!string.IsNullOrWhiteSpace(themeIds))
            {
                var themeIdList = themeIds.Split(',')
                    .Select(id => int.TryParse(id.Trim(), out var parsed) ? parsed : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                if (themeIdList.Count != 0)
                {
                    query = query.Where(i => i.IdeaThemes.Any(it => themeIdList.Contains(it.ThemeId)));
                }
            }

            var ideas = await query
                .Select(idea => new IdeaDto
                {
                    Id = idea.Id,
                    Title = idea.Title,
                    Description = idea.Description,
                    CreatedAt = idea.CreatedAt,
                    LastModifiedAt = idea.LastModifiedAt,
                    Themes = idea.IdeaThemes.Select(it => new ThemeDto
                    {
                        Id = it.Theme.Id,
                        Name = it.Theme.Name
                    }).ToList()
                })
                .ToListAsync();

            return Ok(ideas);
        }


        // GET: api/ideas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IdeaDto>> GetIdea(int id)
        {
            var ideaDto = await _context.Ideas
                .AsNoTracking()
                .Where(i => i.Id == id)
                .Select(idea => new IdeaDto  
                {
                    Id = idea.Id,
                    Title = idea.Title,
                    Description = idea.Description,
                    CreatedAt = idea.CreatedAt,
                    LastModifiedAt = idea.LastModifiedAt,
                    Themes = idea.IdeaThemes.Select(it => new ThemeDto
                    {
                        Id = it.Theme.Id,
                        Name = it.Theme.Name
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (ideaDto == null)
            {
                return NotFound(new { message = $"Idea con ID {id} non trovata" });
            }

            return Ok(ideaDto);
        }



        // POST: api/ideas
        [HttpPost]
        public async Task<ActionResult<IdeaDto>> CreateIdea(CreateIdeaDto createDto)
        {
            var idea = new Idea
            {
                Title = createDto.Title,
                Description = createDto.Description,
                CreatedAt = DateTime.Now,
                LastModifiedAt = DateTime.Now
            };

            _context.Ideas.Add(idea);
            await _context.SaveChangesAsync();

           
            if (createDto.ThemeIds.Count != 0)
            {
                var validThemeIds = await _context.Themes
                    .Where(t => createDto.ThemeIds.Contains(t.Id))
                    .Select(t => t.Id)
                    .ToListAsync();

                var invalidThemeIds = createDto.ThemeIds.Except(validThemeIds).ToList();

                if (invalidThemeIds.Count != 0)
                {
                    _context.Ideas.Remove(idea);
                    await _context.SaveChangesAsync();

                    return BadRequest(new
                    {
                        message = "Temi non validi forniti",
                        invalidIds = invalidThemeIds
                    });
                }

                foreach (var themeId in validThemeIds)
                {
                    _context.IdeaThemes.Add(new IdeaTheme
                    {
                        IdeaId = idea.Id,
                        ThemeId = themeId
                    });
                }
                await _context.SaveChangesAsync();
            }

            var ideaDto = await _context.Ideas
                .AsNoTracking()
                .Where(i => i.Id == idea.Id)
                .Select(i => new IdeaDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    CreatedAt = i.CreatedAt,
                    LastModifiedAt = i.LastModifiedAt,
                    Themes = i.IdeaThemes.Select(it => new ThemeDto
                    {
                        Id = it.Theme.Id,
                        Name = it.Theme.Name
                    }).ToList()
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetIdea), new { id = idea.Id }, ideaDto);
        }


        // PUT: api/ideas/5
        // PUT: api/ideas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIdea(int id, UpdateIdeaDto updateDto)
        {
            var idea = await _context.Ideas
                .Include(i => i.IdeaThemes)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (idea == null)
            {
                return NotFound(new { message = $"Idea con ID {id} non trovata" });
            }

            idea.Title = updateDto.Title;
            idea.Description = updateDto.Description;
            idea.LastModifiedAt = DateTime.Now;

            if (updateDto.ThemeIds != null)
            {
                var currentThemeIds = idea.IdeaThemes.Select(it => it.ThemeId).ToList();

                var newThemeIds = updateDto.ThemeIds;

                var themesToRemove = idea.IdeaThemes
                    .Where(it => !newThemeIds.Contains(it.ThemeId))
                    .ToList();

                foreach (var themeToRemove in themesToRemove)
                {
                    idea.IdeaThemes.Remove(themeToRemove);
                }

                var themeIdsToAdd = newThemeIds
                    .Except(currentThemeIds)
                    .ToList();

                //verifivo l'esistenza del temi da aggiungere
                foreach (var themeId in themeIdsToAdd)
                {
                    idea.IdeaThemes.Add(new IdeaTheme
                    {
                        IdeaId = idea.Id,
                        ThemeId = themeId
                    });
                }
            }

            // 4. Salviamo tutto (EF Core capisce sia l'update dei campi che le modifiche alle relazioni)
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Gestione errori DB (opzionale ma consigliata)
                return StatusCode(500, new { message = "Errore durante il salvataggio", details = ex.Message });
            }

            return NoContent();
        }


        // DELETE: api/ideas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIdea(int id)
        {
            var idea = await _context.Ideas.FindAsync(id);

            if (idea == null)
            {
                return NotFound(new { message = $"Idea con ID {id} non trovata" });
            }

            _context.Ideas.Remove(idea);
            await _context.SaveChangesAsync();

            return NoContent();
        }


         //"Extra"

        // POST: api/ideas/5/themes/3
        [HttpPost("{ideaId}/themes/{themeId}")]
        public async Task<IActionResult> AddThemeToIdea(int ideaId, int themeId)
        {
            var ideaExists = await _context.Ideas.AnyAsync(i => i.Id == ideaId);
            if (!ideaExists)
            {
                return NotFound(new { message = $"Idea con ID {ideaId} non trovata" });
            }

            var themeExists = await _context.Themes.AnyAsync(t => t.Id == themeId);
            if (!themeExists)
            {
                return NotFound(new { message = $"Tema con ID {themeId} non trovato" });
            }

            var linkExists = await _context.IdeaThemes
                .AnyAsync(it => it.IdeaId == ideaId && it.ThemeId == themeId);

            if (linkExists)
            {
                return Conflict(new { message = "Il tema è già associato a questa idea" });
            }

            // Crea il collegamento
            _context.IdeaThemes.Add(new IdeaTheme
            {
                IdeaId = ideaId,
                ThemeId = themeId
            });

            var idea = await _context.Ideas.FindAsync(ideaId);
            idea!.LastModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ideas/5/themes/3
        [HttpDelete("{ideaId}/themes/{themeId}")]
        public async Task<IActionResult> RemoveThemeFromIdea(int ideaId, int themeId)
        {
            var ideaTheme = await _context.IdeaThemes
                .FirstOrDefaultAsync(it => it.IdeaId == ideaId && it.ThemeId == themeId);

            if (ideaTheme == null)
            {
                return NotFound(new { message = "Collegamento tema-idea non trovato" });
            }

            _context.IdeaThemes.Remove(ideaTheme);

            var idea = await _context.Ideas.FindAsync(ideaId);
            if (idea != null)
            {
                idea.LastModifiedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
