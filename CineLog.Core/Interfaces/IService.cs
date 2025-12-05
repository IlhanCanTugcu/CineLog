using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CineLog.Core.Interfaces
{
    // IService katmanı, Controller'ın (Web) muhatap olacağı tek yerdir.
    public interface IService<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        Task RemoveAsync(T entity);
        Task UpdateAsync(T entity);
    }
}


/*
 
Not: Burada Remove ve Update'i de Task (Async) yaptık. 
Çünkü ileride buralara iş mantığı eklersek (Örn: Mail atma) asenkron olması gerekecek. 

 */