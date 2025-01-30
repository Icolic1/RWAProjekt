namespace WebAPI.Models
{
    public class Theme
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation properties
        public ICollection<HeritageTheme> HeritageThemes { get; set; }
    }
}
