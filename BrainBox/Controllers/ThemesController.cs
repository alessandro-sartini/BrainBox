using BrainBox.Data;
using BrainBox.Models;
using BrainBox.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrainBox.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThemesController : ControllerBase
    {
        private readonly BrainBoxDbContext _context;

        public ThemesController(BrainBoxDbContext context)
        {
            _context = context;
        }

        // GET: api/themes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ThemeDto>>> GetThemes()
        {
            return Ok(await _context.Themes
                .AsNoTracking()
                .Select(t => new ThemeDto
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync());
        }

        // GET: api/themes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ThemeDto>> GetTheme(int id)
        {
            var themeDto = await _context.Themes
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new ThemeDto
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .FirstOrDefaultAsync();

            if (themeDto == null)
            {
                return NotFound(new { message = $"Tema con ID {id} non trovato" });
            }

            return Ok(themeDto);
        }

        // POST: api/themes
        [HttpPost]
        public async Task<ActionResult<ThemeDto>> CreateTheme(ThemeCreateDto createDto)
        {
            // Verifica che non esista già un tema con lo stesso nome
            var exists = await _context.Themes
                .AnyAsync(t => t.Name.ToLower().Trim() == createDto.Name.ToLower().Trim());

            if (exists)
            {
                return Conflict(new { message = $"Un tema con nome '{createDto.Name}' esiste già" });
            }

            var theme = new Theme
            {
                Name = createDto.Name
            };

            _context.Themes.Add(theme);
            await _context.SaveChangesAsync();

            var themeDto = new ThemeDto
            {
                Id = theme.Id,
                Name = theme.Name
            };

            return CreatedAtAction(nameof(GetTheme), new { id = theme.Id }, themeDto);
        }

        // PUT: api/themes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTheme(int id, ThemeUpdateDto updateDto)
        {
            var theme = await _context.Themes.FindAsync(id);

            if (theme == null)
            {
                return NotFound(new { message = $"Tema con ID {id} non trovato" });
            }

            //Verifica duplicati
            var duplicateExists = await _context.Themes
                .AnyAsync(t => t.Name.ToLower().Trim() == updateDto.Name.ToLower().Trim() && t.Id != id);

            if (duplicateExists)
            {
                return Conflict(new { message = $"Un tema con nome '{updateDto.Name}' esiste già" });
            }

            theme.Name = updateDto.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/themes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTheme(int id)
        {
            var theme = await _context.Themes.FindAsync(id);

            if (theme == null)
            {
                return NotFound(new { message = $"Tema con ID {id} non trovato" });
            }

           

            _context.Themes.Remove(theme);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
