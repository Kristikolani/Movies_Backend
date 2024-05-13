namespace Movies.Models.DTO;

public class RegistrationRequestDTO
{
    public string UserName { get; set; }                    
    public string Email { get; set; }
    public string Password { get; set; }
    public string Rights { get; set; }
}