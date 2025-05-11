namespace Application.PlantPosts.DTOs
{
    public class PostReadDTO
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string UserNickName { get; set; } = string.Empty;
        public string UserAvatar { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string Background { get; set; } = string.Empty;
        public int NumberOfUpvote { get; set; }
        public int NumberOfDevote { get; set; }
        public int NumberOfComment { get; set; }
        public int NumberOfShare { get; set; }
        public string CreatedDate { get; set; } = string.Empty;
        public DateTime CreatedDateDatetime { get; set; }
        public string Title { get; set; } = string.Empty;
        public IEnumerable<string> Images { get; set; } = new List<string>();
    }
}
