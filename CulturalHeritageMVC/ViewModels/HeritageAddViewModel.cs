using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CulturalHeritageMVC.ViewModels
{
    public class HeritageAddViewModel
    {
        public string Name { get; set; }=null!;
        public string? Description { get; set; }
        public string Location { get; set; } = null!;
        public int Year { get; set; }
        [Required]
        public int NationalMinorityId { get; set; }
        public List<SelectListItem> NationalMinorities { get; set; } = new List<SelectListItem>();
    }
}