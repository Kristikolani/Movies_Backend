using System.Linq.Expressions;
using Movies.Models;

namespace Movies.Repository.IRepository;

public interface IReviewRepository : IRepository<Review>
{
    Task<Review> UpdateAsync(Review entity);
    Task<IEnumerable<Review>> SearchAsync(string? query, string? includeProperties=null,int pageSize=0,int pageNumber=1, Expression<Func<Review, object>>? orderBy = null, bool orderByDescending = false);
    Task<int> GetTotalCountAsync();

}