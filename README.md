# 🎬 CineLog - Kişisel Sinema Arşivi ve Analiz Platformu

> **CineLog**, sinemaseverlerin izledikleri veya izleyecekleri yapımları takip etmelerini sağlayan, **OMDB API** entegrasyonlu, **N-Katmanlı Mimari** ile geliştirilmiş modern bir web uygulamasıdır.

---

## 📖 Proje Hakkında

CineLog, standart bir "To-Do" uygulamasının ötesine geçerek, dış dünyadan (OMDB) anlık veri çeken, bu veriyi yerel veritabanında işleyen ve kullanıcıya özel analizler sunan "Full-Stack" bir projedir. 

Kullanıcılar **Glassmorphism** tasarım diliyle hazırlanmış modern arayüzde filmleri arayabilir, detaylarını inceleyebilir, kendi koleksiyonlarına ekleyebilir ve izleme alışkanlıklarına dair (Favori tür, yönetmen, ortalama puan vb.) istatistiksel raporlar alabilirler.

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

Proje, sürdürülebilirlik ve temiz kod prensipleri gözetilerek **Onion Architecture (Soğan Mimarisi)** benzeri bir N-Katmanlı yapı ile kurgulanmıştır.

```mermaid
graph TD;
    WebUI-->Service;
    Service-->Data;
    Data-->Core;
    Service-->Core;
    WebUI-->Core;

1.CineLog.Core: Projenin kalbi. Entity'ler, Interface'ler ve DTO'lar burada bulunur. Başka hiçbir katmana bağımlı değildir.
2.CineLog.Data: Veritabanı erişim katmanı. DbContext, Migrations ve Repository implementasyonları buradadır.
3.CineLog.Service: İş mantığı (Business Logic) katmanı. API haberleşmesi (ImdbService), Validasyonlar ve veri işleme buradadır.
4.CineLog.Web: Kullanıcının etkileşime girdiği katman. Controller'lar, View'lar ve statik dosyalar buradadır.