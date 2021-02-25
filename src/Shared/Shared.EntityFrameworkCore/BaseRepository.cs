using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.EntityFrameworkCore.Interfaces;

namespace Shared.EntityFrameworkCore
{
    public abstract class BaseRepository<TDbContext, TEntity> : IBaseRepository<TEntity>
        where TEntity : class
        where TDbContext : DbContext
    {
        private readonly ILogger<BaseRepository<TDbContext, TEntity>> _logger;
        private readonly TDbContext _context;

        public BaseRepository(ILogger<BaseRepository<TDbContext, TEntity>> logger,
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

        public async Task<Option<TEntity>> GetAsync<TKey>(TKey key, CancellationToken token = default)
        {
            return await _context.Set<TEntity>().FindAsync(key, token);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken token = default)
        {
            return await _context.Set<TEntity>().AnyAsync(expression);
        }

        public async Task<Option<TEntity>> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> expression, CancellationToken token = default)
        {
            return await _context.Set<TEntity>().SingleOrDefaultAsync(expression, token);
        }

        public async Task<Unit> AddAsync(TEntity data, CancellationToken token = default)
        {
            await _context.Set<TEntity>().AddAsync(data, token);
            await _context.SaveChangesAsync();
            return Unit.Default;
        }

        public async Task<Unit> AddRangeAsync(TEntity[] data, CancellationToken token = default)
        {
            await _context.Set<TEntity>().AddRangeAsync(data, token);
            await _context.SaveChangesAsync(token);
            return Unit.Default;
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

        public async Task<Unit> DeleteAsync<TKey>(TKey data, CancellationToken token = default)
        {
            var entity = await _context.Set<TEntity>().FindAsync(data, token);
            _context.Set<TEntity>().Remove(entity);
            return await _context
                .SaveChangesAsync(token)
                .Map(_ => Unit.Default);
        }

        public async Task<Unit> DeleteRangeAsync<TKey>(TKey[] data, CancellationToken token = default)
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