using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Models;

public class User
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity) ]
    public int Id { get; set; }
    public string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Today;
    public ICollection<Item>? Items { get; set; }
    public ICollection<Review>? Reviews { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    public string Username { get; set; }
    public string Subscription { get; set; } = "Basic";
    public string Rights { get; set; } = "Guest";
    public string Password { get; set; }
}