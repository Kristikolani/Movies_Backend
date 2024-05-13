using System.Linq.Expressions;
using Movies.Models;

namespace Movies.Repository.IRepository;

public interface ICommentRepository : IRepository<Comment>
{
    Task<Comment> UpdateAsync(Comment entity);
    Task<IEnumerable<Comment>> SearchAsync(string? query, string? includeProperties=null,int pageSize=0,int pageNumber=1, Expression<Func<Comment, object>>? orderBy = null, bool orderByDescending = false);
    Task<int> GetTotalCountAsync();

}