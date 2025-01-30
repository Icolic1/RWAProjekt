using Microsoft.AspNetCore.Mvc.Rendering;

namespace CulturalHeritageMVC.ViewModels
{
    public class HeritageFilterAndPagingViewModel
    {
        public List<HeritageListViewModel> Heritages { get; set; } = new List<HeritageListViewModel>();
        public string? Search { get; set; }
        public int? NationalMinorityId { get; set; }
        public List<SelectListItem> NationalMinorities { get; set; } = new List<SelectListItem>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

