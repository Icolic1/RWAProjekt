namespace WebAPI.Models
{
    public class UserHeritageComment
    {
        public int UserId { get; set; }
        public User User { get; set; }=null!;

        public int HeritageId { get; set; }
        public Heritage Heritage { get; set; } = null!;

        public string Comment { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
