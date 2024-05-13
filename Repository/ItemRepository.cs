using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.Models;

namespace Movies.Repository.IRepository;

public class ItemRepository : Repository<Item>, IItemRepository
{
    private readonly ApplicationDbContext _db;
    public ItemRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
    
    
    public async Task<Item> UpdateAsync(Item entity)
    {
        _db.Items.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
    
    public async Task<IEnumerable<Item>> SearchAsync(string? searchText, string? includeProperties = null,int pageSize=0,int pageNumber=1, Expression<Func<Item, object>>? orderBy = null, bool orderByDescending = false)
    {
        
        IQueryable<Item> query = dbSet;
        
        if (includeProperties != null)
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }
        if (orderBy!=null)
        {
            query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        }
        if (searchText != null)
        {
            query = query
                .Where(item => item.Title.Contains(searchText));
        }
        if (pageSize>0)
        {
            if (pageSize>100)
            {
                pageSize = 100;
            }

            query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
        }

        return await query.ToListAsync();

    }
    
    
    public async Task<int> GetTotalCountAsync()
    {
        return await _db.Items.CountAsync();
    }
}