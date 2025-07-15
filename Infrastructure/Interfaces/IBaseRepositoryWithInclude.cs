using System.Linq.Expressions;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IBaseRepositoryWithInclude<T1, T2> : IBaseRepository<T1, T2>
{
    Task<List<T1>> GetAllIncludingAsync(params Expression<Func<T1, object>>[] includeProperties);
    Task<T1?> GetByIdIncludingAsync(int id, params Expression<Func<T1, object>>[] includeProperties);


}
