using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CineLog.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
            // Id'ye göre getir
        Task<T?> GetByIdAsync(int id);

            // Hepsini getir
        Task<IEnumerable<T>> GetAllAsync();

            // Şarta göre getir (Örn: Yılı 2010 olanlar). 
            // IQueryable döndürüyoruz ki veritabanına gitmeden sorguya devam edebilelim.
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        void Remove(T entity);
        void Update(T entity);
    }
}


/*
---------------------------------------------------------------------------------

    IGenericRepository arayüzü (interface), projemizin "Anayasasıdır". 
Diğer sınıflara "Eğer veritabanı işi yapacaksan, şu kurallara uymak zorundasın" dediğimiz yerdir.

---------------------------------------------------------------------------------

    Neden Task?: Veritabanına gitmek zaman alan bir işlemdir (I/O). 
Programın donmaması için Asenkron (Async) yapıyoruz.

---------------------------------------------------------------------------------

    Expression<Func<T, bool>>: Bu ifade aslında yazdığımız Lambda sorgusudur.

Örnek: x => x.Year == "2010"

Bu kod şunu der: "Bana bir T (film) ver, ben sana bunun doğru olup olmadığını (bool) söyleyeyim."

---------------------------------------------------------------------------------

    IQueryable (Neden List değil?): Eğer List döndürseydik, veritabanından tüm veriyi çeker, sonra 
filtrelerdik. (Performans kaybı). IQueryable ise "Henüz veritabanına gitme, sorguyu hazırlamaya 
devam ediyorum" demektir. Böylece sen bu metodun ucuna .OrderBy() veya .Take(5) eklediğinde, 
EF Core bunların hepsini birleştirip tek bir SQL sorgusu olarak veritabanına yollar.
 
--------------------------------------------------------------------------------- 

    Soru: Neden bunlar Task (Async) değil de düz void?
    Cevap: Çok ince bir detaydır. Entity Framework'te Remove ve Update metotları veritabanına 
anında gitmez. Sadece hafızadaki o nesnenin durumunu (State) "Silinecek" veya "Güncellenecek" 
olarak işaretler. Asıl işlem biz SaveChanges dediğimizde yapılır. O yüzden bu işaretleme işlemi 
milisaniyeler sürer, bekletmeye (await) gerek yoktur.

--------------------------------------------------------------------------------- 

 */
/*

Interface'ler (arayüzler) ilk bakışta "gereksiz iş yükü" gibi görünür. "Zaten GenericRepository diye bir sınıfım var, 
neden direkt onu kullanmıyorum da araya bir IGenericRepository sıkıştırıyorum?" diyebilirsin.

Bunun cevabı 3 ana maddede gizli: Özgürlük, Güvenlik ve Test.



1. Özgürlük: "Priz ve Fiş" Mantığı (Loose Coupling)
Duvardaki elektrik prizini düşün (Interface). Prizin arkasında hangi kablo var, elektrik barajdan mı geliyor yoksa 
güneş panelinden mi, senin ütünü ilgilendirmez. Ütünün (Web Projesi) tek derdi o iki deliğe (Metotlara) uymaktır.

Eğer Interface kullanmazsan; ütünün kablosunu direkt duvarın içindeki elektrik hattına lehimlemiş olursun (Tightly Coupled).

Sorun: Yarın elektrik hattını değiştirmek istersen duvarı kırman gerekir. Bizim Projede: Eğer Web katmanında direkt 
GenericRepository (SQL Server) kullanırsan, yarın patron gelip "Artık SQL değil, Oracle veya MongoDB kullanacağız" 
dediğinde, projede 100 farklı yerde kod değiştirmen gerekir. Interface ile: Sadece Program.cs içinde tek bir 
satırı değiştirirsin.

Eski: services.AddScoped<IRepo, SqlRepo>();
Yeni: services.AddScoped<IRepo, MongoRepo>();

Web projesinin ruhu bile duymaz, çünkü o sadece IRepo tanır.



2. Test Edilebilirlik: "Sahte Veri" (Mocking)
Bir yazılımı profesyonel yapan şey testlerdir. Diyelim ki "Sepete Ekle" butonunun mantığını test edeceksin. 
Interface Yoksa: Testi çalıştırdığında kodun gidip gerçek veritabanına bağlanmaya çalışır. Test veritabanını 
kirletir, yavaştır ve internet koparsa test patlar.

Interface Varsa: IGenericRepositoryyi taklit eden sahte bir sınıf (FakeRepository) yaparsın. Bu sahte sınıf 
veritabanına gitmez, hafızadaki bir listeyi (List) kullanır. Koda dersin ki: "Test yaparken bu sahteyi kullan."
Kod IGenericRepository üzerinden konuştuğu için karşısındakinin sahte olduğunu anlamaz.




3. Çoklu Çalışma (Dependency Injection)
Büyük projelerde backend ekibi ve veritabanı ekibi ayrı olabilir. Sen (Backendci), IGenericRepository 
arayüzüne bakarak kodunu yazarsın (Service katmanını bitirirsin). "Elimde GetById var mı? Var. Tamam ben bunu kullanayım."
O sırada veritabanı ekibi henüz GenericRepository kodlarını (içini) yazmamış olabilir.Interface bir Sözleşmedir. 
Sen sözleşmeye güvenerek işine devam edersin, diğer taraf işini bitirince sadece fişi takmak kalır. 



Kısacası Interface kullanmak; "Ben bugün SQL Server ile evliyim ama yarın boşanıp MongoDB ile evlenebilirim, 
kodlarımın buna hazır olması lazım" demektir.

 */