using Movies.Models.DTO;

namespace Movies.Models;

public class CommentDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserDTO User { get; set; }
    public int ItemId { get; set; }
    public ItemDTO Item { get; set; }
    public string Text { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public DateTime CreatedDate { get; set; }
}