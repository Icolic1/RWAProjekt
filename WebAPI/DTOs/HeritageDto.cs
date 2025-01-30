namespace WebAPI.DTOs
{
    public class HeritageDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public int Year { get; set; }
        public int NationalMinorityId { get; set; }
        public IEnumerable<string>? Themes { get; set; } // Lista naziva tema
        
    }
}
