using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class NationalMinority
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }

        // nav svojstva
        public ICollection<Heritage>? Heritages { get; set; }
    }
}
