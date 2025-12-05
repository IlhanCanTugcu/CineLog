using CineLog.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CineLog.Service.Services
{
    public class Service<T> : IService<T> where T : class
    {
        // Service katmanı, veritabanı işleri için Repository'i kullanır.
        private readonly IGenericRepository<T> _repository;

        public Service(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(T entity)
        {
            await _repository.AddAsync(entity);
            // SaveChanges zaten Repo içinde var, tekrar çağırmıyoruz.
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task RemoveAsync(T entity)
        {
            _repository.Remove(entity);
            await Task.CompletedTask; // Metot async olduğu için formalite.
        }

        public async Task UpdateAsync(T entity)
        {
            _repository.Update(entity);
            await Task.CompletedTask;
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _repository.Where(expression);
        }

    }
}



/*
 
Özetle Neden Async ve Await Kullanıyoruz?

Hız Değil, Kapasite: async kodu tek bir işlemi hızlandırmaz (hatta minik bir gecikme ekler). 
Ama sunucunun aynı anda daha fazla kullanıcıyı kaldırmasını sağlar (Scalability).

Kaynak Verimliliği: İşlemciyi (CPU) boş yere bekletmemek için.

Modern Standart: I/O işlemleri (Veritabanı, Dosya okuma, API isteği) olan her yerde async kullanmak 
artık endüstri standardıdır.
 
 */
