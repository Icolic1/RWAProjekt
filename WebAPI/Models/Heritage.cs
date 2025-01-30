using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Heritage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Heritage Name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }=null!;

        [Required(ErrorMessage = "Location is required.")]
        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Range(1000, 3000, ErrorMessage = "Year must be between 1000 and 3000.")]
        [Display(Name = "Year of Creation")]
        public int Year { get; set; }

        [Required(ErrorMessage = "National Minority is required.")]
        [Display(Name = "National Minority")]
        public int NationalMinorityId { get; set; }
        // Navigation properties
        public NationalMinority? NationalMinority { get; set; }
        public ICollection<HeritageTheme>? HeritageTheme{ get; set; }
        public ICollection<UserHeritageComment>? UserHeritageComment { get; set; } = new List<UserHeritageComment>();
    }
    
    


}
