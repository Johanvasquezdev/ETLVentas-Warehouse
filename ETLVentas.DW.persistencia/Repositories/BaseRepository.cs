using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ETLVentas.DW.application.Interfaces.Repositories;
using ETLVentas.DW.application.Models.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ETLVentas.DW.persistencia.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly DWContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly ILogger _logger;

        public BaseRepository(DWContext context, ILogger<BaseRepository<T>> logger)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _logger = logger;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<OperationResult> AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return OperationResult.Ok("Entidad guardada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error insertando registro en {EntityName}", typeof(T).Name);
                return OperationResult.Fail($"Error insertando registro en {typeof(T).Name}", ex);
            }
        }

        public async Task<OperationResult> AddRangeAsync(IEnumerable<T> entities)
        {
            try
            {
                await _dbSet.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                return OperationResult.Ok($"{entities.Count()} registros insertados correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error insertando rango de registros en {EntityName}", typeof(T).Name);
                return OperationResult.Fail($"Error insertando múltiples registros en {typeof(T).Name}", ex);
            }
        }

        public async Task<OperationResult> UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                return OperationResult.Ok("Entidad actualizada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando registro en {EntityName}", typeof(T).Name);
                return OperationResult.Fail($"Error actualizando registro en {typeof(T).Name}", ex);
            }
        }

        public async Task<OperationResult> RemoveAsync(T entity)
        {
            try
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                return OperationResult.Ok("Entidad eliminada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando registro en {EntityName}", typeof(T).Name);
                return OperationResult.Fail($"Error eliminando registro en {typeof(T).Name}", ex);
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}
