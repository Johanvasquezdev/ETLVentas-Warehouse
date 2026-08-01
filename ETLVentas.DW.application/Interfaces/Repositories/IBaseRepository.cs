using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ETLVentas.DW.application.Models.Results;

namespace ETLVentas.DW.application.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByIdAsync(int id);
        Task<OperationResult> AddAsync(T entity);
        Task<OperationResult> AddRangeAsync(IEnumerable<T> entities);
        Task<OperationResult> UpdateAsync(T entity);
        Task<OperationResult> RemoveAsync(T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
