using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace Movies.Models.DTO;

public class ItemCreateDTO
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public int? ReleaseYear { get; set; }
    public string? Category { get; set; }
    public string? Director { get; set; }
    public IList<string>? Cast { get; set; }
    public int? RunningTime { get; set; }
    public string? Quality { get; set; }
    public string? Country { get; set; }
    public string? Cover { get; set; }
    public string[]? Photos { get; set; }
    public string? Video { get; set; }
    public string? Link { get; set; }
    [Precision(18, 2)]
    public decimal? Rating { get; set; }
    public string? Status { get; set; }
    public IList<string>? Genres { get; set; }
}