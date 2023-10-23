using AuthUser.Application.Interfaces.Repositories;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthUser.Infrastructure.Persistence.Repositories
{
    public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : class
    {
        protected readonly ISession Session;

        public GenericRepositoryAsync(ISession session)
        {
            Session = session;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await Session.SaveAsync(entity).ConfigureAwait(false);
            await Session.FlushAsync().ConfigureAwait(false);
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(T entity)
        {
            await Session.DeleteAsync(entity).ConfigureAwait(false);
            await Session.FlushAsync().ConfigureAwait(false);
            return true;
        }

        public virtual async Task<IQueryable<T>> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return (await (Session.Query<T>()
                .Where(expression).ToListAsync().ConfigureAwait(false))).AsQueryable();
        }

        public virtual async Task<IQueryable<T>> GetAllAsync()
        {
            return (await Session.Query<T>().ToListAsync().ConfigureAwait(false)).AsQueryable();
        }

        public virtual async Task<IQueryable<T>> GetPagedResponseAsync(int pageNumber, int pageSize)
        {
            return (await Session.Query<T>().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false)).AsQueryable();
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            await Session.SaveOrUpdateAsync(entity).ConfigureAwait(false);
            await Session.FlushAsync().ConfigureAwait(false);
            return entity;
        }
    }
}