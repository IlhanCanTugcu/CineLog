using CineLog.Core.Interfaces;
using CineLog.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CineLog.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context; // Veritabanı bağlantımız
        private readonly DbSet<T> _dbSet; // İlgili tablo (Movies vs.)
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }
    }
}


/*
 ---------------------------------------------------------------------------------

    _context: Hatırlarsan AppDbContext sınıfını yazmıştık. O bizim SQL Server ile olan bağlantımızdı.

    _dbSet: Burası çok kritik. Eğer bu sınıf GenericRepository<Movie> olarak çağrılırsa, 
_context.Set<Movie>() çalışır ve _dbSet otomatik olarak Movies tablosuna kilitlenir. Kodun geri kalanında 
_dbSet.Add() dediğimizde, EF Core bunu gidip Movies tablosuna ekleyeceğini bilir.

---------------------------------------------------------------------------------
 
    Ekleme İşlemi (AddAsync) : Mantık: Gelen veriyi (entity) ilgili tablonun (DbSet) hafızasına ekler.
    Dikkat: Hala SQL'e "INSERT INTO..." komutu gitmedi. Sadece hafızada "Bu eklenecek, 
haberin olsun" denildi. Veritabanına yazma işi SaveChanges() çağrılınca olacak (Bunu Service katmanında yapacağız).
 
 ---------------------------------------------------------------------------------
    
    Silme ve Güncelleme (Neden void?) "Neden veritabanı işi yapıyoruz ama Async değil?"

    Cevap: Entity Framework (EF) Core akıllı bir araçtır. Remove dediğinde veritabanına gidip o satırı silmez.
Sadece hafızasındaki o nesnenin üzerine kırmızı bir etiket yapıştırır: State = Deleted. Bu etiketleme işlemi 
RAM üzerinde milisaniyeden kısa sürdüğü için, programı bekletmeye (await etmeye) gerek yoktur. 
Asıl silme işlemi yine SaveChanges() dendiğinde topluca yapılır.
    
 ---------------------------------------------------------------------------------

    Okuma İşlemleri (Get):

    FindAsync: Bu metot çok özeldir. Önce hafızaya (Cache) bakar, "Bu ID elimde var mı?" diye. 
Varsa veritabanına bile gitmez. Yoksa SQL'e SELECT * FROM Movies WHERE Id=1 sorgusunu atar.

 ---------------------------------------------------------------------------------

    Sorgulama (Where): Burası Interface'de konuştuğumuz IQueryable mantığı.

Sen bu metodu çağırdığında (Örn: repo.Where(x => x.Year > 2000)), veritabanına hiçbir şey gitmez.
Sadece SQL sorgusunun "WHERE" kısmı hazırlanır ve beklemede kalır. Ne zaman ki veriyi ToList() veya 
FirstOrDefault() ile istersin, o zaman sorgu tamamlanır ve SQL Server'a yolculuk başlar.

 ---------------------------------------------------------------------------------
    
    Özetle GenericRepository Ne Yapar?

    Bu sınıf, Entity Framework'ün karmaşık metotlarını (DbContext, DbSet, EntityState vs.) bizim için 
paketler ve basit 5-6 komuta indirger. Biz Service katmanında kod yazarken artık 
context.Movies.Where(...).State = ... gibi karmaşık işlerle uğraşmayacağız. Şunu diyeceğiz:

_repository.AddAsync(film) -> Ekle.

_repository.Remove(film) -> Sil.

---------------------------------------------------------------------------------

 */