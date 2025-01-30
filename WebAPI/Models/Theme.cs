namespace WebAPI.Models
{
    public class Theme
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        // nav svojstva 
        public ICollection<HeritageTheme> HeritageThemes { get; set; } = new List<HeritageTheme>();
    }
}
