using CulturalHeritageMVC.ViewModels;
using System.ComponentModel.DataAnnotations;
using WebAPI.Models;

namespace CulturalHeritageMVC.Models
{
    public class HeritageEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;
        public int Year { get; set; }

        [Required]
        public int NationalMinorityId { get; set; }
        public List<NationalMinority> NationalMinorities { get; set; } = new List<NationalMinority>();

        // Nova kolekcija za komentare
        public List<UserHeritageCommentViewModel> Comments { get; set; } = new List<UserHeritageCommentViewModel>();
    }
}
