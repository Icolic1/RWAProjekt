using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using WebAPI.Data;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Controllers
{
    
        [Route("api/heritage")]
        [ApiController]
        public class HeritageController : ControllerBase
        {
            private readonly CulturalHeritageDbContext _context;
        private readonly ILogRepository _logRepository;

        public HeritageController(CulturalHeritageDbContext context, ILogRepository logRepository)
            {
                _context = context;
                _logRepository = logRepository;
        }

            // GET: api/heritage

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllHeritages(int page = 1, int count = 10)
        {
            try
            {
                var heritages = await _context.Heritage
                    .Select(h => new
                    {
                        h.Id,
                        h.Name,
                        h.Description,
                        h.Location,
                        h.Year,
                        NationalMinority = h.NationalMinority.Name,
                        Themes = h.HeritageTheme.Select(ht => ht.Theme.Name)
                    })
                    .Skip((page - 1) * count)
                    .Take(count)
                    .ToListAsync();

                return Ok(heritages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving heritages.", Error = ex.Message });
            }
        }


        // GET: api/heritage/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<HeritageDto>> GetHeritageById(int id)
        {
            try
            {
                var heritage = await _context.Heritage
                    .Include(h => h.NationalMinority) // Dohvati povezanu nacionalnu manjinu
                    .Include(h => h.HeritageTheme) // Dohvati bridge entitet HeritageTheme
                        .ThenInclude(ht => ht.Theme) // Dohvati povezane teme
                    .FirstOrDefaultAsync(h => h.Id == id);

                if (heritage == null)
                {
                    return NotFound(new { Message = "Heritage not found." });
                }

                var heritageDto = new HeritageDto
                {

                    Name = heritage.Name,
                    Description = heritage.Description,
                    Location = heritage.Location,
                    Year = heritage.Year,
                    NationalMinorityId = heritage.NationalMinority.Id,
                    Themes = heritage.HeritageTheme.Select(ht => ht.Theme.Name) // Mapira nazive tema
                };

                return Ok(heritageDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving the heritage.", Error = ex.Message });
            }
        }

        // POST: api/heritage
        [HttpPost]
        public async Task<ActionResult<HeritageDto>> CreateHeritage(HeritageDto heritageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Provjera postoji li NationalMinority s navedenim ID-jem
            var nationalMinorityExists = await _context.NationalMinority
                .AnyAsync(nm => nm.Id == heritageDto.NationalMinorityId);

            if (!nationalMinorityExists)
            {
                return BadRequest(new { Message = "Invalid NationalMinorityId. The specified National Minority does not exist." });
            }

            // Početak transakcije
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Kreiraj novi entitet Heritage
                var heritage = new Heritage
                {
                    Name = heritageDto.Name,
                    Description = heritageDto.Description,
                    Location = heritageDto.Location,
                    Year = heritageDto.Year,
                    NationalMinorityId = heritageDto.NationalMinorityId
                };

                _context.Heritage.Add(heritage);
                await _context.SaveChangesAsync();

                // Dodaj ili kreiraj teme
                foreach (var themeName in heritageDto.Themes.Distinct()) // Ukloni duplikate
                {
                    // Dohvati temu iz baze ili je kreiraj
                    var theme = await _context.Theme.FirstOrDefaultAsync(t => t.Name == themeName);
                    if (theme == null)
                    {
                        theme = new Theme { Name = themeName };
                        _context.Theme.Add(theme);
                        await _context.SaveChangesAsync();
                    }

                    // Poveži temu s Heritage
                    var heritageTheme = new HeritageTheme
                    {
                        HeritageId = heritage.Id,
                        ThemeId = theme.Id
                    };

                    _context.HeritageTheme.Add(heritageTheme);
                }

                // Sačuvaj sve promene
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Priprema HeritageDto za odgovor
                var heritageDtoResponse = new HeritageDto
                {
                    Name = heritage.Name,
                    Description = heritage.Description,
                    Location = heritage.Location,
                    Year = heritage.Year,
                    NationalMinorityId = heritage.NationalMinorityId, // Direktno koristimo ID iz heritage
                    Themes = heritage.HeritageTheme.Select(ht => ht.Theme.Name)
                };

                return CreatedAtAction(nameof(GetHeritageById), new { id = heritage.Id }, heritageDtoResponse);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "An error occurred while creating the heritage.", Error = ex.Message });
            }
        }

        // PUT: api/heritage/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHeritage(int id, HeritageDto heritageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Pronađi postojeći entitet Heritage
                var heritage = await _context.Heritage
                    .Include(h => h.HeritageTheme)
                    .ThenInclude(ht => ht.Theme)
                    .FirstOrDefaultAsync(h => h.Id == id);

                if (heritage == null)
                {
                    return NotFound(new { Message = "Heritage not found." });
                }

                // Ažuriraj osnovne atribute
                heritage.Name = heritageDto.Name;
                heritage.Description = heritageDto.Description;
                heritage.Location = heritageDto.Location;
                heritage.Year = heritageDto.Year;
                heritage.NationalMinorityId = heritageDto.NationalMinorityId;

                // Obradi povezane teme
                if (heritageDto.Themes != null)
                {
                    // Postojeće teme
                    var existingThemes = heritage.HeritageTheme.Select(ht => ht.Theme.Name).ToList();

                    // Dodaj nove teme
                    foreach (var themeName in heritageDto.Themes.Except(existingThemes))
                    {
                        var theme = await _context.Theme.FirstOrDefaultAsync(t => t.Name == themeName);
                        if (theme == null)
                        {
                            theme = new Theme { Name = themeName };
                            _context.Theme.Add(theme);
                            await _context.SaveChangesAsync();
                        }

                        // Dodaj novu vezu između Heritage i Theme
                        heritage.HeritageTheme.Add(new HeritageTheme
                        {
                            HeritageId = heritage.Id,
                            ThemeId = theme.Id
                        });
                    }

                    // Ukloni nepostojeće teme
                    foreach (var themeName in existingThemes.Except(heritageDto.Themes))
                    {
                        var heritageTheme = heritage.HeritageTheme.FirstOrDefault(ht => ht.Theme.Name == themeName);
                        if (heritageTheme != null)
                        {
                            _context.HeritageTheme.Remove(heritageTheme);
                        }
                    }
                }

                // Sačuvaj promene
                await _context.SaveChangesAsync();

                // Loguj akciju
                LogAction($"Updated heritage with ID = {heritage.Id}");

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HeritageExists(id))
                {
                    return NotFound(new { Message = "Heritage not found." });
                }

                return StatusCode(500, new { Message = "An error occurred while updating the heritage." });
            }
        }


        // DELETE: api/heritage/{id}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHeritage(int id)
        {
            if (_context.Heritage==null)
            {
                return NotFound();
            }
            // Pronađi entitet
            var heritage = await _context.Heritage.FindAsync(id);

            if (heritage == null)
            {
                return NotFound(new { Message = "Heritage not found." });
            }

            try
            {
                // Proveri stanje entiteta
              

                // Obriši heritage
                _context.Heritage.Remove(heritage);

                // Sačuvaj promene
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while deleting the heritage.",
                    Error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }


        // GET: api/heritage/search
        [HttpGet("search")]
            public async Task<ActionResult<IEnumerable<Heritage>>> SearchHeritages(string name, int page = 1, int count = 10)
            {
                try
                {
                    var heritages = await _context.Heritage
                        .Where(h => h.Name.Contains(name))
                        .Skip((page - 1) * count)
                        .Take(count)
                        .ToListAsync();

                    return Ok(heritages);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Message = "An error occurred while searching for heritages.", Error = ex.Message });
                }
            }

            // GET: api/logs/get/{N}
            [HttpGet("logs/get/{N}")]
            public IActionResult GetLogs(int N = 10)
            {
                // Retrieve the last N logs from your logging mechanism
                return Ok(_logRepository.GetLastLogs(N));
            }

            // GET: api/logs/count
            [HttpGet("logs/count")]
            public IActionResult GetLogCount()
            {
                return Ok(_logRepository.GetLogCount());
            }

            private void LogAction(string message)
            {
            _logRepository.AddLog(new Log
                {
                    Timestamp = DateTime.UtcNow,
                    Level = "INFO",
                    Message = message
                });
            }

            private bool HeritageExists(int id)
            {
                return _context.Heritage.Any(e => e.Id == id);
            }
        }
 }

