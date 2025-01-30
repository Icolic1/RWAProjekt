using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models;

namespace CulturalHeritageMVC.Controllers
{
    public class NationalMinorityController : Controller
    {
        private readonly CulturalHeritageDbContext _context;

        public NationalMinorityController(CulturalHeritageDbContext context)
        {
            _context = context;
        }

        // List
        [HttpGet]
        public IActionResult List()
        {
            var minorities = _context.NationalMinority
                .Include(nm => nm.Heritages)
                .ToList();

            return View(minorities);
        }

        // Add - GET
        [HttpGet]
        public IActionResult Add()
        {
            return View(new NationalMinority());
        }

        // Add - POST
        [HttpPost]
        public IActionResult Add(NationalMinority model)
        {
            if (ModelState.IsValid)
            {
                _context.NationalMinority.Add(model);
                _context.SaveChanges();
                return RedirectToAction("List");
            }

            return View(model);
        }

        // Edit - GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var minority = _context.NationalMinority.FirstOrDefault(nm => nm.Id == id);
            if (minority == null) return NotFound();

            return View(minority);
        }

        // Edit - POST
        [HttpPost]
        public IActionResult Edit(NationalMinority model)
        {
            if (ModelState.IsValid)
            {
                _context.NationalMinority.Update(model);
                _context.SaveChanges();
                return RedirectToAction("List");
            }

            return View(model);
        }

        // Delete - POST
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var minority = _context.NationalMinority.FirstOrDefault(nm => nm.Id == id);
            if (minority == null) return NotFound();

            _context.NationalMinority.Remove(minority);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}

