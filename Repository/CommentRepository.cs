using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.Models;
using Movies.Repository.IRepository;

namespace Movies.Repository;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    private readonly ApplicationDbContext _db;

    public CommentRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task<Comment> UpdateAsync(Comment entity)
    {
        _db.Comments.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<Comment>> SearchAsync(string? searchText, string? includeProperties = null,int pageSize=0,int pageNumber=1, Expression<Func<Comment, object>>? orderBy = null, bool orderByDescending = false)
    {
        IQueryable<Comment> query = dbSet;
       

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
                .Where(comment => comment.Text.Contains(searchText));
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
        return await _db.Comments.CountAsync();
    }

}