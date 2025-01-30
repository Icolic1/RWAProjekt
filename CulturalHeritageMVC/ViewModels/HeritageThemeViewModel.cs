using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CulturalHeritageMVC.ViewModels
{
    public class HeritageThemeViewModel
    {
        public int Id { get; set; }
        public int HeritageId { get; set; }
        public int ThemeId { get; set; }

        // Select lists for dropdowns
        public List<SelectListItem> Heritages { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Themes { get; set; } = new List<SelectListItem>();
    }
}

