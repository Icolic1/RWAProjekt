namespace CulturalHeritageMVC.ViewModels
{
    public class HeritageListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public int Year { get; set; }
        public string? NationalMinorityName { get; set; }
        public List<UserHeritageCommentViewModel> Comments { get; set; } = new List<UserHeritageCommentViewModel>();
    }
}

