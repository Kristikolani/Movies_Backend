using Microsoft.EntityFrameworkCore;

namespace Movies.Models.DTO;

public class ReviewUpdateDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ItemId { get; set; }
    public string Text { get; set; }
    [Precision(18, 2)]
    public decimal Rating { get; set; }
}