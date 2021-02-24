using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.EntityFrameworkCore.Interfaces;

namespace Shared.EntityFrameworkCore
{
    public class BaseRepository<TDbContext, TEntity, TKey> : IBaseRepository<TEntity, TKey>
        where TEntity : Entity<TKey>
        where TDbContext : DbContext
    {
        private readonly ILogger<BaseRepository<TDbContext, TEntity, TKey>> _logger;
        private readonly TDbContext _context;

        public BaseRepository(ILogger<BaseRepository<TDbContext, TEntity, TKey>> logger,
            TDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IImmutableList<TEntity>> ListAllAsync(CancellationToken token = default)
        {
            return (await _context.Set<TEntity>().AsTracking().ToListAsync(token))
                .ToImmutableList();
        }

        public async Task<IImmutableList<TEntity>> ListAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> query, CancellationToken token = default)
        {
            return (await query(_context.Set<TEntity>().AsQueryable()).AsTracking().ToListAsync(token))
                .ToImmutableList();
        }

        public async Task<IImmutableList<TResponse>> ListAsync<TResponse>(Func<IQueryable<TEntity>, IQueryable<TResponse>> query, CancellationToken token = default)
        {
            return (await query(_context.Set<TEntity>().AsNoTracking().AsQueryable()).ToListAsync(token))
                .ToImmutableList();
        }

        public async Task<Option<TEntity>> Get(TKey key, CancellationToken token = default)
        {
            return await _context.Set<TEntity>().FindAsync(key, token);
        }

        public async Task<TKey> AddAsync(TEntity data, CancellationToken token = default)
        {
            await _context.Set<TEntity>().AddAsync(data, token);
            await _context.SaveChangesAsync();
            return data.Id;
        }

        public async Task<TKey[]> AddRangeAsync(TEntity[] data, CancellationToken token = default)
        {
            await _context.Set<TEntity>().AddRangeAsync(data, token);
            await _context.SaveChangesAsync(token);
            return data.Select(x => x.Id).ToArray();
        }

        public Task<Unit> UpdateAsync(TEntity data, CancellationToken token = default)
        {
            _context.Set<TEntity>().Update(data);
            return _context.SaveChangesAsync(token)
                .Map(_ => Unit.Default);
        }

        public Task<Unit> UpdateRangeAsync(TEntity[] data, CancellationToken token = default)
        {
            _context.Set<TEntity>().UpdateRange(data);
            return _context.SaveChangesAsync(token)
                .Map(_ => Unit.Default);
        }

        public async Task<Unit> DeleteAsync(TKey data, CancellationToken token = default)
        {
            var entity = await _context.Set<TEntity>().FindAsync(data, token);
            _context.Set<TEntity>().Remove(entity);
            return await _context
                .SaveChangesAsync(token)
                .Map(_ => Unit.Default);
        }

        public async Task<Unit> DeleteRangeAsync(TKey[] data, CancellationToken token = default)
        {
            foreach (var key in data)
            {
                var entity = await _context.Set<TEntity>().FindAsync(key, token);
                _context.Set<TEntity>().Remove(entity);
            }

            return await _context
                .SaveChangesAsync(token)
                .Map(_ => Unit.Default);
        }
    }
}