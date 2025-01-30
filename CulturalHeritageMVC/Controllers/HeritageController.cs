
using CulturalHeritageMVC.Models;
using CulturalHeritageMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPI.Data;

using WebAPI.Models;


namespace CulturalHeritageMVC.Controllers
{
    [Authorize]
    public class HeritageController : Controller
    {
        private readonly CulturalHeritageDbContext _context;

        public HeritageController(CulturalHeritageDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> List(string search = "", int? nationalMinorityId = null, int page = 1, int pageSize = 10)
        {
            // Upit za Heritage entitete, uključujući NationalMinority, UserHeritageComment i User
            var query = _context.Heritage
                .Include(h => h.NationalMinority)
                .Include(h => h.UserHeritageComment)
                    .ThenInclude(uhc => uhc.User) // Učitaj i korisnike koji su ostavili komentare
                .AsQueryable();

            // Kombinacija pretrage i filtera
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(h => h.Name.Contains(search));
            }

            if (nationalMinorityId.HasValue)
            {
                query = query.Where(h => h.NationalMinorityId == nationalMinorityId.Value);
            }

            // Ukupan broj zapisa za paging
            var totalCount = await query.CountAsync();

            // Paging i dohvaćanje podataka
            var heritages = await query
                .OrderBy(h => h.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Priprema ViewModel-a
            var model = new HeritageFilterAndPagingViewModel
            {
                Heritages = heritages.Select(h => new HeritageListViewModel
                {
                    Id = h.Id,
                    Name = h.Name,
                    Description = h.Description,
                    Location = h.Location ?? "N/A",
                    Year = h.Year,
                    NationalMinorityName = h.NationalMinority?.Name ?? "No National Minority",

                    // Mapiranje komentara
                    Comments = h.UserHeritageComment.Select(c => new UserHeritageCommentViewModel
                    {
                        Username = c.User.Username, // Pretpostavka da User model ima Username
                        Comment = c.Comment,
                        CreatedAt = c.CreatedAt
                    }).ToList()

                }).ToList(),

                Search = search,
                NationalMinorityId = nationalMinorityId,
                NationalMinorities = await _context.NationalMinority
                    .Select(nm => new SelectListItem
                    {
                        Value = nm.Id.ToString(),
                        Text = nm.Name
                    }).ToListAsync(),

                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View(model);
        }







        [HttpGet]
        public IActionResult Add()
        {
            var model = new HeritageAddViewModel
            {
                NationalMinorities = _context.NationalMinority
                    .Select(nm => new SelectListItem
                    {
                        Value = nm.Id.ToString(),
                        Text = nm.Name
                    })
                    .ToList()
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Add(HeritageAddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Ponovno popunite popis nacionalnih manjina
                model.NationalMinorities = await _context.NationalMinority
                    .Select(nm => new SelectListItem
                    {
                        Value = nm.Id.ToString(),
                        Text = nm.Name
                    })
                    .ToListAsync();

                return View(model);
            }

            // Provjera za jedinstvenost imena
            if (await _context.Heritage.AnyAsync(h => h.Name == model.Name))
            {
                ModelState.AddModelError("Name", "A heritage with this name already exists."); // Dodavanje greške

                // Ponovno popunite popis nacionalnih manjina
                model.NationalMinorities = await _context.NationalMinority
                    .Select(nm => new SelectListItem
                    {
                        Value = nm.Id.ToString(),
                        Text = nm.Name
                    })
                    .ToListAsync();

                return View(model); // Vracanje pogreske korisniku
            }

            // Ako nema gresaka, stvori novi Heritage objekt
            var heritage = new Heritage
            {
                Name = model.Name,
                Description = model.Description,
                Location = model.Location,
                Year = model.Year,
                NationalMinorityId = model.NationalMinorityId
            };

            _context.Heritage.Add(heritage);
            await _context.SaveChangesAsync();

            return RedirectToAction("List", "Heritage");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var heritage = _context.Heritage
                .Include(h => h.UserHeritageComment)
                    .ThenInclude(uhc => uhc.User) // Učitaj i korisnika koji je ostavio komentar
                .FirstOrDefault(h => h.Id == id);

            if (heritage == null)
            {
                return NotFound();
            }

            var model = new HeritageEditViewModel
            {
                Id = heritage.Id,
                Name = heritage.Name,
                Description = heritage.Description,
                Location = heritage.Location ?? string.Empty, // Popravak za CS8601
                Year = heritage.Year,
                NationalMinorityId = heritage.NationalMinorityId,
                NationalMinorities = _context.NationalMinority.ToList(), // Puni listu nacionalnih manjina

                // Mapiranje komentara na ViewModel
                Comments = heritage.UserHeritageComment.Select(c => new UserHeritageCommentViewModel
                {
                    
                    Username = c.User.Username, // Pretpostavljam da User ima Username
                    Comment = c.Comment,
                    CreatedAt = c.CreatedAt
                }).ToList()
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult Edit(HeritageEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Ako validacija ne uspije, ponovno popuni listu nacionalnih manjina
                model.NationalMinorities = _context.NationalMinority.ToList();
                return View(model);
            }

            var heritage = _context.Heritage.FirstOrDefault(h => h.Id == model.Id);
            if (heritage == null)
            {
                return NotFound();
            }

            // Ažuriranje Heritage entiteta
            heritage.Name = model.Name;
            heritage.Description = model.Description;
            heritage.Location = model.Location;
            heritage.Year = model.Year;
            heritage.NationalMinorityId = model.NationalMinorityId;

            _context.SaveChanges();

            return RedirectToAction("List", "Heritage");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var heritage = await _context.Heritage.FindAsync(id);
            if (heritage == null) return NotFound();

            _context.Heritage.Remove(heritage);
            await _context.SaveChangesAsync();
            return RedirectToAction("List");
        }
        [HttpPost]
        [Authorize]
        public IActionResult AddComment(int HeritageId, string Comment)
        {
            // Dohvati ID korisnika iz claimova
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            // Dummy provjera - samo jedan hardkodirani user (ako nema User tabele)
            var user = new { Id = userId, Username = "admin" }; // Ako ima User tabela, koristi `_context.Users.FirstOrDefault(u => u.Id == userId)`

            if (user == null)
            {
                return Unauthorized();
            }

            var newComment = new UserHeritageComment
            {
                HeritageId = HeritageId,
                UserId = userId,
                Comment = Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserHeritageComment.Add(newComment);
            _context.SaveChanges();

            return Json(new
            {
                username = user.Username, // Prikaz hardkodiranog username-a
                comment = newComment.Comment,
                createdAt = newComment.CreatedAt.ToString("yyyy-MM-dd HH:mm")
            });
        }



    }
}
