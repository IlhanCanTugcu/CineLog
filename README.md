# 🎬 CineLog - Kişisel Sinema Arşivi ve Analiz Platformu

> **CineLog**, sinema tutkunlarının izledikleri veya izlemeyi planladıkları yapımları kayıt altına alıp yönetebildikleri, **OMDB API** entegrasyonu ile güçlendirilmiş, **N-Katmanlı Mimari (N-Tier Architecture)** prensiplerine göre tasarlanmış modern ve ölçeklenebilir bir **ASP.NET Core MVC** projesidir.
---

## 📖 Proje Hakkında

CineLog, standart bir "To-Do" uygulamasının ötesine geçerek, dış dünyadan (OMDB) anlık veri çeken, bu veriyi yerel veritabanında işleyen ve kullanıcıya özel analizler sunan "Full-Stack" bir projedir. 

Projenin temel amacı, kullanıcılara sadece statik bir liste sunmak değil; **canlı veri akışı**, **görsel zenginlik** ve **kişisel analizler** ile yaşayan bir deneyim yaşatmaktır. Kullanıcılar, milyonlarca film arasından saniyeler içinde arama yapabilir, filmlerin detaylı bilgilerine (Poster, Yıl, Tür, IMDB Puanı vb.) erişebilir ve tek bir tıkla bu verileri kendi yerel veritabanlarına kaydedebilirler. 

---

## ✨ Öne Çıkan Özellikler

### 🔍 **Akıllı Arama Motoru**
* OMDB API entegrasyonu ile milyonlarca film, dizi ve oyun arasında anlık arama.
* Arama sonuçlarında poster, yıl ve tür önizlemesi.

### 🔐 **Güvenli Üyelik Sistemi**
* **ASP.NET Core Identity** altyapısı.
* Kayıt Ol / Giriş Yap / Çıkış Yap döngüsü.
* Kullanıcıya özel veri izolasyonu (Herkes sadece kendi listesini görür).

### 📂 **Kişisel Koleksiyon Yönetimi**
* Filmleri "Arşivim" listesine ekleme.
* Listeden film silme.
* Mükerrer kayıt kontrolü (Aynı film iki kere eklenemez).

### 📊 **İstatistik ve Analiz (Dashboard)**
* **Kişisel Analiz:** Toplam film sayısı, en sevilen tür, favori yönetmen ve ortalama IMDB puanı hesaplaması.
* **Global Trendler:** Platform genelinde diğer kullanıcıların en çok listelediği "Top 4" filmi görebilme.

### 🎨 **Modern UI/UX**
* **Bootswatch Cyborg** teması (Dark Mode).
* Özel CSS ile **Glassmorphism** (Buzlu Cam) efektleri.
* Responsive (Mobil Uyumlu) tasarım.

---

## 🛠 Teknoloji Yığını

| Alan | Teknoloji | Açıklama |
| :--- | :--- | :--- |
| **Backend** | .NET 8 (C#) | Ana geliştirme platformu. |
| **Database** | MS SQL Server | İlişkisel veritabanı yönetimi. |
| **ORM** | Entity Framework Core | Code First yaklaşımı ile veri erişimi. |
| **Identity** | ASP.NET Core Identity | Kimlik doğrulama ve yetkilendirme. |
| **API** | HttpClient & Newtonsoft | OMDB API ile RESTful haberleşme. |
| **Frontend** | Razor Views | Server-side rendering. |
| **Styling** | Bootstrap 5 & CSS3 | Bootswatch tema ve özel animasyonlar. |

---

## 🏗 Mimari Yapı

Proje, sürdürülebilirlik, test edilebilirlik ve temiz kod (Clean Code) prensipleri gözetilerek **Onion Architecture (Soğan Mimarisi)** benzeri, gevşek bağlı (loosely coupled) bir N-Katmanlı yapı ile kurgulanmıştır.

**Katmanların Bağımlılık Akışı:**
`Web (UI) -> Service (Logic) -> Data (Database) -> Core (Entities)`

Proje 4 ana katmandan oluşmaktadır:

1.  **CineLog.Core (Merkez):** Projenin kalbidir ve başka hiçbir katmana bağımlı değildir. Tüm katmanlar burayı referans alır.
    * *İçerik:* Varlıklar (`Entities`), Arayüzler (`Interfaces`), Veri Transfer Objeleri (`DTOs`).
2.  **CineLog.Data (Veri Erişim):** Veritabanı ile iletişimden sorumludur. Core katmanındaki soyutlamaları (Interface) uygular.
    * *İçerik:* `DbContext`, `Migrations`, `Repository` implementasyonları, `Entity Framework Core` konfigürasyonları.
3.  **CineLog.Service (İş Mantığı):** Uygulamanın kurallarının işletildiği yerdir. Controller ile Data katmanı arasındaki köprüdür.
    * *İçerik:* Validasyonlar, API Haberleşme Servisleri (`ImdbService`), İş mantığı metodları.
4.  **CineLog.Web (Sunum):** Kullanıcının etkileşime girdiği en dış katmandır.
    * *İçerik:* `Controllers`, `Views` (Razor), `ViewModels`, Statik Dosyalar (CSS/JS).

