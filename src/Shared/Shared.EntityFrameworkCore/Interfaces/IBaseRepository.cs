using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;

namespace Shared.EntityFrameworkCore.Interfaces
{
    public interface IBaseRepository<TEntity, TKey>
        where TEntity : Entity<TKey>
    {
        Task<IImmutableList<TEntity>> ListAllAsync(CancellationToken token = default);
        Task<IImmutableList<TEntity>> ListAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> query, CancellationToken token = default);
        Task<IImmutableList<TResponse>> ListAsync<TResponse>(Func<IQueryable<TEntity>, IQueryable<TResponse>> query, CancellationToken token = default);
        Task<Option<TEntity>> Get(TKey key, CancellationToken token = default);
        
        Task<TKey> AddAsync(TEntity data, CancellationToken token = default);
        Task<TKey[]> AddRangeAsync(TEntity[] data, CancellationToken token = default);
        Task<Unit> UpdateAsync(TEntity data, CancellationToken token = default);
        Task<Unit> UpdateRangeAsync(TEntity[] data, CancellationToken token = default);
        Task<Unit> DeleteAsync(TKey data, CancellationToken token = default);
        Task<Unit> DeleteRangeAsync(TKey[] data, CancellationToken token = default);
    }
}