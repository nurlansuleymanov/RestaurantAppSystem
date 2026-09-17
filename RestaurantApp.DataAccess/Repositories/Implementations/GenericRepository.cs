using Microsoft.EntityFrameworkCore;
using RestaurantApp.DataAccess.Context;
using RestaurantApp.DataAccess.Repositories.Interfaces;
using RestaurantApp.Entity.Entities.Common;

namespace RestaurantApp.DataAccess.Repositories.Implementations;

public class GenericRepository<T> : IGenericRepository<T>
    where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _table;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _table = _context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _table
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _table
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(T entity)
    {
        await _table.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _table.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _table.Remove(entity);
        await _context.SaveChangesAsync();
    }
}