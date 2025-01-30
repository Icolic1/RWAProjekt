namespace WebAPI.Models
{
    public class UserHeritageComment
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int HeritageId { get; set; }
        public Heritage Heritage { get; set; }

        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
