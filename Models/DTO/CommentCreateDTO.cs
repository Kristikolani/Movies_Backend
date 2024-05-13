using Movies.Models.DTO;

namespace Movies.Models;

public class CommentCreateDTO
{
    public int UserId { get; set; }
    public int ItemId { get; set; }
    public string Text { get; set; }
}