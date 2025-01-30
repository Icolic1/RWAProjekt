using CulturalHeritageMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data;

public class UserController : Controller
{
    private readonly CulturalHeritageDbContext _context;

    public UserController(CulturalHeritageDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult UpdateUser()
    {
        // pretpostavi da je admin korisnik jedini korisnik
        var admin = _context.User.FirstOrDefault(u => u.Username == "admin");

        if (admin == null || string.IsNullOrEmpty(admin.Username))
        {
            throw new Exception("Admin user data is invalid or not found.");
        }
        if (admin == null) return NotFound();

        var model = new UserViewModel
        {
            Username = admin.Username,
            Email = admin.Email,
            FirstName = admin.FirstName,
            LastName = admin.LastName,
            PhoneNumber = admin.PhoneNumber??""
        };

        return View(model);
    }

    [HttpPost]
    public JsonResult UpdateUser([FromBody] UserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        // pretpostavi da je admin korisnik jedini korisnik
        var admin = _context.User.FirstOrDefault(u => u.Username == "admin");

        if (admin == null) return Json(new { success = false, message = "User not found." });

        admin.Username = model.Username;
        admin.Email = model.Email;
        admin.FirstName = model.FirstName;
        admin.LastName = model.LastName;
        admin.PhoneNumber = model.PhoneNumber;

        _context.SaveChanges();

        return Json(new { success = true, message = "Profile updated successfully." });
    }
}
