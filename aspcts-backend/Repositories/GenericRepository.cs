using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using aspcts_backend.Data;
using aspcts_backend.Repositories.Interface;

namespace aspcts_backend.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }
    
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
    
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.Where(expression).ToListAsync();
    }
    
    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.FirstOrDefaultAsync(expression);
    }
    
    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }
    
    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        return entities;
    }
    
    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }
    
    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
    
    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
    
    public virtual async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.AnyAsync(expression);
    }
    
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? expression = null)
    {
        return expression == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(expression);
    }
    }
}