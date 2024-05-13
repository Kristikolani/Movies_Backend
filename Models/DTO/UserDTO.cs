namespace Movies.Models;

public class UserDTO
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public ICollection<Item>? Items { get; set; }
    public ICollection<Review>? Reviews { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    public string Subscription { get; set; }
    public string Rights { get; set; }
}