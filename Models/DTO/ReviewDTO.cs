using Microsoft.EntityFrameworkCore;

namespace Movies.Models.DTO;

public class ReviewDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserDTO User { get; set; }
    public int ItemId { get; set; }
    public ItemDTO Item { get; set; }
    public string Text { get; set; }
    [Precision(18, 2)]
    public decimal Rating { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public DateTime CreatedDate { get; set; }
}