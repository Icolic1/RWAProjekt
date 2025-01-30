using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data;
using WebAPI.Models;
using CulturalHeritageMVC.ViewModels;
using Microsoft.EntityFrameworkCore;

public class HeritageThemeController : Controller
{
    private readonly CulturalHeritageDbContext _context;

    public HeritageThemeController(CulturalHeritageDbContext context)
    {
        _context = context;
    }

    // List
    [HttpGet]
    public IActionResult List()
    {
        var heritageThemes = _context.HeritageTheme
            .Include(ht => ht.Heritage)
            .Include(ht => ht.Theme)
            .ToList();

        return View(heritageThemes);
    }

    // Add - GET
    [HttpGet]
    public IActionResult Add()
    {
        var model = new HeritageThemeViewModel
        {
            Heritages = _context.Heritage.Select(h => new SelectListItem
            {
                Value = h.Id.ToString(),
                Text = h.Name
            }).ToList(),
            Themes = _context.Theme.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name
            }).ToList()
        };

        return View(model);
    }

    // Add - POST
    [HttpPost]
    public IActionResult Add(HeritageThemeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Reload dropdowns in case of error
            model.Heritages = _context.Heritage.Select(h => new SelectListItem
            {
                Value = h.Id.ToString(),
                Text = h.Name
            }).ToList();
            model.Themes = _context.Theme.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name
            }).ToList();

            return View(model);
        }

        // Provjera da li kombinacija vec postoji
        if (_context.HeritageTheme.Any(ht => ht.HeritageId == model.HeritageId && ht.ThemeId == model.ThemeId))
        {
            ModelState.AddModelError(string.Empty, "This Heritage-Theme combination already exists.");
            model.Heritages = _context.Heritage.Select(h => new SelectListItem
            {
                Value = h.Id.ToString(),
                Text = h.Name
            }).ToList();
            model.Themes = _context.Theme.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name
            }).ToList();

            return View(model);
        }

        var heritageTheme = new HeritageTheme
        {
            HeritageId = model.HeritageId,
            ThemeId = model.ThemeId
        };

        _context.HeritageTheme.Add(heritageTheme);
        _context.SaveChanges();

        return RedirectToAction("List");
    }

    // Edit - GET
    [HttpGet]
    public IActionResult Edit(int heritageId, int themeId)
    {
        var heritageTheme = _context.HeritageTheme
            .FirstOrDefault(ht => ht.HeritageId == heritageId && ht.ThemeId == themeId);
        if (heritageTheme == null) return NotFound();

        var model = new HeritageThemeViewModel
        {
            HeritageId = heritageTheme.HeritageId,
            ThemeId = heritageTheme.ThemeId,
            Heritages = _context.Heritage.Select(h => new SelectListItem
            {
                Value = h.Id.ToString(),
                Text = h.Name
            }).ToList(),
            Themes = _context.Theme.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name
            }).ToList()
        };

        return View(model);
    }

    // Edit - POST
    [HttpPost]
    public IActionResult Edit(HeritageThemeViewModel model, int originalHeritageId, int originalThemeId)
    {
        if (!ModelState.IsValid)
        {
            // Reload dropdowns in case of error
            model.Heritages = _context.Heritage.Select(h => new SelectListItem
            {
                Value = h.Id.ToString(),
                Text = h.Name
            }).ToList();
            model.Themes = _context.Theme.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name
            }).ToList();

            return View(model);
        }

        // Dohvati HeritageTheme pomocu originalnog slozenog ključa
        var heritageTheme = _context.HeritageTheme
            .FirstOrDefault(ht => ht.HeritageId == originalHeritageId && ht.ThemeId == originalThemeId);

        if (heritageTheme == null) return NotFound();

        // Ažuriraj slozeni kljuc
        heritageTheme.HeritageId = model.HeritageId;
        heritageTheme.ThemeId = model.ThemeId;

        _context.SaveChanges();

        return RedirectToAction("List");
    }

    // Delete
    [HttpPost]
    public IActionResult Delete(int heritageId, int themeId)
    {
        var heritageTheme = _context.HeritageTheme
            .FirstOrDefault(ht => ht.HeritageId == heritageId && ht.ThemeId == themeId);
        if (heritageTheme == null) return NotFound();

        _context.HeritageTheme.Remove(heritageTheme);
        _context.SaveChanges();

        return RedirectToAction("List");
    }
}
