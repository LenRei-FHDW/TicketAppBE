using Microsoft.EntityFrameworkCore;

namespace TicketAPI.Data.Repositories;

public interface IRepository<TEntity , in TIdentifier> 
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(TIdentifier id);
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    Task DeleteAsync(TIdentifier id);
    Task DeleteEntityAsync(TEntity entity);
    
    Task<bool> ExistsAsync(TIdentifier id);
}

public class Repository<TEntity,TIdentifier>: IRepository<TEntity, TIdentifier> where TEntity : class
{
    protected readonly TicketApiDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(TicketApiDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }
    
    public async Task<IEnumerable<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();
    public async Task<TEntity> GetByIdAsync(TIdentifier id)
    {
        return await _dbSet.FindAsync(id) ?? throw new KeyNotFoundException();
    }
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        var entry = await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _context.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TIdentifier id)
    {
        if (await ExistsAsync(id))
        {
            var entity = await _dbSet.FindAsync(id);
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException();
        }
    }
    
    public async Task DeleteEntityAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(TIdentifier id)
    {
        return await _dbSet.FindAsync(id) != null;
    }
}