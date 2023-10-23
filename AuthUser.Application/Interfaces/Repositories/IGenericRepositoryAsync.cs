using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthUser.Application.Interfaces.Repositories
{
    public interface IGenericRepositoryAsync<T> where T : class
    {
        Task<IQueryable<T>> GetAllAsync();

        Task<IQueryable<T>> FindByCondition(Expression<Func<T, bool>> expression);

        Task<IQueryable<T>> GetPagedResponseAsync(int pageNumber, int pageSize);

        Task<T> AddAsync(T entity);

        Task<T> UpdateAsync(T entity);

        Task<bool> DeleteAsync(T entity);
    }
}