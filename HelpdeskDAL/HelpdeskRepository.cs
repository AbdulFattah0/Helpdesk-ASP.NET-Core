using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using System;

using System.Threading.Tasks;
using HelpdeskDAL;

public class HelpdeskRepository<T> : IRepository<T> where T : HelpdeskEntity
{
    private readonly HelpdeskContext _context;

    public HelpdeskRepository(HelpdeskContext context)
    {
        _context = context;
    }

    // Parameterless constructor for default instantiation
    public HelpdeskRepository()
    {
        var options = new DbContextOptionsBuilder<HelpdeskContext>()
            .UseInMemoryDatabase("HelpdeskDb") // Or switch to another real database
            .Options;
        _context = new HelpdeskContext(options);
    }

    // Repository methods...
    public async Task<T?> GetOne(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public async Task<List<T>> GetAll()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<int> Add(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return await _context.SaveChangesAsync();
    }

    public async Task<UpdateStatus> Update(T entity)
    {
        _context.Set<T>().Update(entity);
        var result = await _context.SaveChangesAsync();
        return result > 0 ? UpdateStatus.Ok : UpdateStatus.Failed;
    }

    public async Task<int> Delete(int id)
    {
        var entity = await GetOne(e => e.Id == id);
        if (entity == null) return 0;

        _context.Set<T>().Remove(entity);
        return await _context.SaveChangesAsync();
    }
}


