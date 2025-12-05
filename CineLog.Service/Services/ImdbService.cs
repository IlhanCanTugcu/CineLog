using CineLog.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using CineLog.Core.DTOs;

namespace CineLog.Service.Services
{
    public class ImdbService : IImdbService
    {
        private readonly HttpClient _httpClient;

        private const string ApiKey = "b6b6e400";
        private const string BaseUrl = "http://www.omdbapi.com/";

        // OPTİMİZASYON: JSON ayarlarını her defasında new'lememek için static yaptık.
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public ImdbService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OmdbDto?> GetMovieByIdAsync(string imdbId)
        {
            // Örnek: http://www.omdbapi.com/?i=tt12345&apikey=xyz
            var response = await _httpClient.GetAsync($"{BaseUrl}?i={imdbId}&apikey={ApiKey}");
            return await ParseResponse(response);
        }

        // OPTİMİZASYON: Bu metot sınıfın hiçbir değişkenini (field) kullanmadığı için 'static' yaptık.
        private static async Task<OmdbDto?> ParseResponse(HttpResponseMessage response)
        {
            // 1. Kontrol: Karşı taraf telefonu açtı mı? (200 OK)
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            // 2. Okuma: Telefondaki sesi kaydet (JSON string olarak al)
            var jsonString = await response.Content.ReadAsStringAsync();

            // 3. Çeviri (Deserialize): En kritik yer!
            // JSON'daki "Title" yazısını al -> DTO'daki 'Title' kutusuna koy.
            // JSON'daki "Plot" yazısını al -> DTO'daki 'Description' kutusuna koy.
            try
            {
                // Yukarıda oluşturduğumuz static _jsonOptions'ı kullanıyoruz.
                return JsonSerializer.Deserialize<OmdbDto>(jsonString, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        public async Task<OmdbSearchResponse?> SearchMoviesAsync(string searchTerm)
        {
            // Dikkat: Burada ?s= kullanıyoruz (Search)
            var response = await _httpClient.GetAsync($"{BaseUrl}?s={searchTerm}&apikey={ApiKey}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            
            try
            {
                return JsonSerializer.Deserialize<OmdbSearchResponse>(jsonString, _jsonOptions);
            }
            catch
            {
                return null;
            }
        }
    }
}

/*
 * ---------------------------------------------------------------------------------------------------------------

    Genel Mantık: Bu Sınıf Ne Yapıyor?
Sen normalde tarayıcını (Chrome) açıp adres çubuğuna şunu yazıyorsun: www.omdbapi.com/?t=Batman&apikey=12345

Karşına karmaşık yazılar (JSON) çıkıyor. İşte ImdbService sınıfı, senin yerine kodun içinden tarayıcı açıp 
o adrese giden ve ekranda yazanları okuyup sana getiren bir robottur.

---------------------------------------------------------------------------------------------------------------
    
    Deserialize (Tersine Çevirme): Senaryo: API sana IKEA'dan demonte bir masa (JSON String) yolladı. 
    Deserialize: O kutuyu açıp parçaları birleştirerek sapasağlam bir masa (OmdbDto Class) haline getirme işlemidir.
 
 ---------------------------------------------------------------------------------------------------------------

    JsonSerializerOptions (Çeviri Kuralları)

Neden var? API bazen "title" (küçük harf), bazen "Title" (büyük harf) yollayabilir.

CaseInsensitive = true: "Büyük küçük harfe takılma, kelime aynıysa eşleştir" demektir.

Static: Bu kural kitabını her film için baştan yazmayalım, duvara asalım herkes oradan okusun (Performans için).
 
 ---------------------------------------------------------------------------------------------------------------
    Akış Şeması (Data Flow): Kod çalıştığında arka planda şu trafik döner:

Sen (Controller): "Bana Batman'i bul."

ImdbService: "Tamam, HttpClient telefonunu alıyorum."

İstek (Request): omdbapi.com adresine git.

OMDB (İnternet): "Al sana Batman verisi (JSON)."

ImdbService: "Bu JSON çok karışık. JsonSerializer tercümanı gel buraya! Bunu C# diline çevir."

Sonuç: Tertemiz, içi dolu bir OmdbDto nesnesi Controller'a döner.
 
 
 
 
 
 */



