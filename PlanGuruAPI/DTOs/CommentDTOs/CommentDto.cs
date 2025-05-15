namespace PlanGuruAPI.DTOs.CommentDTOs
{
    public class CommentDto
    {
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Message { get; set; }
        public string CreatedAt { get; set; }
        public int NumberOfUpvote { get; set; }
        public int NumberOfDevote { get; set; }
        public bool HasUpvoted { get; set; } // Thêm thuộc tính này
        public bool HasDevoted { get; set; } // Thêm thuộc tính này

        public List<CommentDto> Replies { get; set; } = new List<CommentDto>(); // Danh sách các bình luận con
    }
}
