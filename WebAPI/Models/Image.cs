namespace WebAPI.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public int HeritageId { get; set; }

        // Navigation properties
        public Heritage Heritage { get; set; }
    }
}
