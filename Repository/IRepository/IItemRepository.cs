using System.Linq.Expressions;
using Movies.Models;

namespace Movies.Repository.IRepository;

public interface IItemRepository : IRepository<Item>
{
    Task<Item> UpdateAsync(Item entity);
    Task<IEnumerable<Item>> SearchAsync(string? query, string? includeProperties=null,int pageSize=0,int pageNumber=1, Expression<Func<Item, object>>? orderBy = null, bool orderByDescending = false);
    Task<int> GetTotalCountAsync();

}