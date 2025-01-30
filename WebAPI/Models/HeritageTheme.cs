using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
    [Table("HeritageTheme")]
    public class HeritageTheme
    {
        public int HeritageId { get; set; }
        public Heritage Heritage { get; set; } = null!;

        public int ThemeId { get; set; }
        public Theme Theme { get; set; } = null!;
    }
}
