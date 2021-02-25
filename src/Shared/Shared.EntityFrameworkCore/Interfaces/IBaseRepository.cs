using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;

namespace Shared.EntityFrameworkCore.Interfaces
{
    public interface IBaseRepository<TEntity>
        where TEntity : class
    {
        Task<IImmutableList<TEntity>> ListAllAsync(CancellationToken token = default);
        Task<IImmutableList<TEntity>> ListAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> query, CancellationToken token = default);
        Task<IImmutableList<TResponse>> ListAsync<TResponse>(Func<IQueryable<TEntity>, IQueryable<TResponse>> query, CancellationToken token = default);
        Task<Option<TEntity>> GetAsync<TKey>(TKey key, CancellationToken token = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken token = default);
        Task<Option<TEntity>> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> expression, CancellationToken token = default);
        
        Task<Unit> AddAsync(TEntity data, CancellationToken token = default);
        Task<Unit> AddRangeAsync(TEntity[] data, CancellationToken token = default);
        Task<Unit> UpdateAsync(TEntity data, CancellationToken token = default);
        Task<Unit> UpdateRangeAsync(TEntity[] data, CancellationToken token = default);
        Task<Unit> DeleteAsync<TKey>(TKey data, CancellationToken token = default);
        Task<Unit> DeleteRangeAsync<TKey>(TKey[] data, CancellationToken token = default);
    }
}