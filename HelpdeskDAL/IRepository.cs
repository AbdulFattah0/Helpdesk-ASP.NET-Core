using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public interface IRepository<T>
    {
        Task<T?> GetOne(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetAll();
        Task<int> Add(T entity);
        Task<UpdateStatus> Update(T entity);
        Task<int> Delete(int id);
    }
}
