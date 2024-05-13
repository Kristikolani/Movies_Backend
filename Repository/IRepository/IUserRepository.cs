using System.Linq.Expressions;
using Movies.Models;
using Movies.Models.DTO;

namespace Movies.Repository.IRepository;

public interface IUserRepository
{
    
        bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<User> Register(RegistrationRequestDTO registrationRequestDTO);
        Task<List<User>> GetAllAsync(Expression<Func<User,bool>>? filter = null,string? includeProperties=null,
                int pageSize=0,int pageNumber=1);
        Task<User> GetAsync(Expression<Func<User, bool>> filter = null,bool tracked=true,string? includeProperties=null);
        
        Task<IEnumerable<User>> SearchAsync(string? query, string? includeProperties=null,int pageSize=0,int pageNumber=1, Expression<Func<User, object>>? orderBy = null, bool orderByDescending = false);
        Task<int> GetTotalCountAsync();

        
        Task<User> UpdateAsync(User entity);

        Task DeleteAsync(User entity);

        Task SaveAsync();
}