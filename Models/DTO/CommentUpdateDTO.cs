namespace Movies.Models;

public class CommentUpdateDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ItemId { get; set; }
    public string Text { get; set; }
}