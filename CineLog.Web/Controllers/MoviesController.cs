using CineLog.Core.DTOs;
using CineLog.Core.Entities;
using CineLog.Core.Interfaces;
using CineLog.Service.Services;
using CineLog.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Recommendations;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CineLog.Web.Controllers
{
    public class MoviesController : Controller
    {
        // Servisimizi çağırıyoruz
        private readonly IImdbService _imdbService; //(Harici API Erişimi)
        private readonly IService<Movie> _movieService; //(Veritabanı Erişimi)
        private readonly IService<UserMovie> _userMovieService; // <--- YENİ: İlişki Servisi
        private readonly UserManager<AppUser> _userManager; // <--- YENİ: Kullanıcı Bulucu
        public MoviesController(IImdbService imdbService, IService<Movie> movieService, UserManager<AppUser> userManager, IService<UserMovie> userMovieService)
        {
            _imdbService = imdbService;
            _movieService = movieService;
            _userManager = userManager;
            _userMovieService = userMovieService;
        }

        // 1. Ana Sayfa (Arama Kutusu)
        public IActionResult Index()
        {
            return View(new OmdbSearchResponse());
        }

        // 2. Arama İşlemi (Listeyi Getirir)
        [HttpPost]
        public async Task<IActionResult> Index(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return View(new OmdbSearchResponse());
            }


            var result = await _imdbService.SearchMoviesAsync(term);
            return View(result);
        }

        // 3. Detay Sayfası (Tek Filme Tıklanınca Çalışır)
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var movieDto = await _imdbService.GetMovieByIdAsync(id);
            // 1. Tüm türleri listeye çevir (Örn: ["Adventure", "Drama", "Western"])
            var genres = movieDto.Genre.Split(new[] { ", " }, StringSplitOptions.None).ToList();

            // 2. Varsayılan arama kelimesi (İlk sıradaki)
            var searchGenre = genres[0];

            // 3. DAHA BELİRLEYİCİ TÜRLER LİSTESİ (Öncelik Sırası)
            // Adventure veya Drama çok geneldir. Eğer filmde aşağıdaki türlerden biri varsa, onu baz alalım.
            var priorityGenres = new List<string> { "Western", "Horror", "Animation", "Sci-Fi", "Fantasy", "War" };

            // Listemizdeki türlerden herhangi biri, öncelikli listede var mı?
            var specificGenre = genres.FirstOrDefault(g => priorityGenres.Contains(g));

            if (specificGenre != null)
            {
                searchGenre = specificGenre; // Adventure yerine Western'i seç!
            }

            var similarMovies = _movieService
                .Where(x => x.Genre.Contains(searchGenre) && x.ImdbId != id)
                .Take(4)
                .ToList();

            var model = new MovieDetailViewModel
            {
                Details = movieDto,
                Recommendations = similarMovies
            };
            return View(model);
        }

        // LİSTEME EKLE (Sadece Giriş Yapanlar)
        [HttpPost]
        [Authorize] // <--- DİKKAT: Giriş yapmayan buraya gelemez, Login'e atılır.
        public async Task<IActionResult> AddToList(string imdbId)
        {
            // 1. ADIM: Şu anki kullanıcıyı bul
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // 2. ADIM: Film veritabanımızda kayıtlı mı?
            // (Önce filmi bulmamız veya oluşturmamız lazım ki ID'si olsun)
            var movie = _movieService.Where(x => x.ImdbId == imdbId).FirstOrDefault();

            if (movie == null)
            {
                // Film yoksa API'den çekip kaydedelim
                var movieDto = await _imdbService.GetMovieByIdAsync(imdbId);
                if (movieDto == null)
                {
                    TempData["Error"] = "Film bulunamadı.";
                    return RedirectToAction("Index");
                }

                movie = new Movie
                {
                    ImdbId = movieDto.ImdbId,
                    Title = movieDto.Title,
                    Year = movieDto.Year,
                    Director = movieDto.Director,
                    Genre = movieDto.Genre,
                    ImdbRating = movieDto.ImdbRating,
                    PosterUrl = movieDto.PosterUrl,
                    Description = movieDto.Description,
                    CreatedDate = DateTime.Now
                };

                // Filmi kaydet (Artık movie.Id oluştu!)
                await _movieService.AddAsync(movie);
            }

            // 3. ADIM: İlişki Kontrolü (Zaten ekli mi?)
            // "Bu kullanıcı (user.Id) bu filmi (movie.Id) daha önce eklemiş mi?"
            var relationExists = _userMovieService
                .Where(x => x.AppUserId == user.Id && x.MovieId == movie.Id)
                .Any();

            if (relationExists)
            {
                TempData["Error"] = "Bu film zaten listenizde var!";
                return RedirectToAction("Details", new { id = imdbId });
            }

            // 4. ADIM: İlişkiyi Kaydet (UserMovie tablosuna yaz)
            var userMovie = new UserMovie
            {
                AppUserId = user.Id,
                MovieId = movie.Id,
                AddedDate = DateTime.Now,
                IsWatched = false
            };

            await _userMovieService.AddAsync(userMovie);

            TempData["Success"] = "Listene eklendi!";
            return RedirectToAction("Details", new { id = imdbId });
        }

        // KULLANICININ LİSTESİ (GET)
        [Authorize] // Sadece giriş yapanlar görebilir
        [HttpGet]
        public async Task<IActionResult> MyList()
        {
            // 1. Aktif kullanıcıyı bul
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // 2. Kullanıcının filmlerini çek
            // .Include(x => x.Movie) -> "UserMovie getirirken bağlı olduğu Film detayını da getir" demek.
            var myMovies = _userMovieService
                .Where(x => x.AppUserId == user.Id)
                .Include(x => x.Movie)
                .OrderByDescending(x => x.AddedDate) // En son eklenen en başta olsun
                .ToList();

            return View(myMovies);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFromList(string imdbId)
        {
            // 1. Giriş yapan kullanıcıyı bul
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // 2. Silinecek Filmi Bul (ImdbId'den bizim ID'ye ulaşmak için)
            var movie = _movieService.Where(x => x.ImdbId == imdbId).FirstOrDefault();
            if (movie == null) return RedirectToAction("MyList");

            // 3. İLİŞKİYİ BUL (UserMovie Tablosundan Çek)
            // "Bu kullanıcının listesindeki bu filmi getir" diyoruz.
            var userMovieRelation = _userMovieService
                .Where(x => x.AppUserId == user.Id && x.MovieId == movie.Id)
                .FirstOrDefault();

            // 4. SİLME İŞLEMİ (Senin bahsettiğin Generic Remove burada!)
            if (userMovieRelation != null)
            {
                // _userMovieService.RemoveAsync -> Repository.Remove -> SaveChanges zinciri çalışır.
                await _userMovieService.RemoveAsync(userMovieRelation);

                TempData["Success"] = "Film listeden başarıyla silindi.";
            }

            return RedirectToAction("MyList");
        }

        // İSTATİSTİKLER SAYFASI (GET)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Stats()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var myList = _userMovieService
                .Where(x => x.AppUserId == user.Id)
                .Include(x => x.Movie)
                .ToList();

            var favoriteGenre = myList
                .SelectMany(x => x.Movie.Genre.Split(", ")) // 1. Parçala ve Yay
                .GroupBy(g => g)                             // 2. Grupla
                .OrderByDescending(grp => grp.Count())       // 3. Sırala
                .Select(grp => grp.Key)                      // 4. İsmi Al
                .FirstOrDefault();                           // 5. Birincisik

            var avgRating = myList.Any()
                ? myList.Where(x => x.Movie.ImdbRating != "N/A")                 // Puanı olmayanları ele
                .Select(x => x.Movie.ImdbRating)                                 // Sadece puanları al ("8.5", "7.2")
                .AsEnumerable()                                                  // Belleğe çek (Parse işlemi için)   
                .Average(r => double.Parse(r.Replace(".", ","))) : 0;            // String -> Double çevir

            var allUserMovie = _userMovieService            // 1. Tüm Veritabanını Tara
                .Where(x => true)
                .Include(x => x.Movie)
                .ToList();

            var mostAdded = allUserMovie
                .GroupBy(x => x.MovieId)                           // 2. Filme Göre Grupla
                .Select(g => new PopularMovieDto
                {
                    Title = g.First().Movie.Title,
                    PosterUrl = g.First().Movie.PosterUrl,
                    ImdbRating = g.First().Movie.ImdbRating,
                    AddCount = g.Count()
                })
                .OrderByDescending(x => x.AddCount)              // 3. Eklenme Sayısına Göre Sırala
                .Take(4)
                .ToList();

            var topDirector = myList
                .GroupBy(x => x.Movie.Director)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            var stats = new StatsViewModel
            {
                TotalMovies = myList.Count,
                FavoriteGenre = favoriteGenre,
                AverageImdb = Math.Round(avgRating, 1),
                MostAddedMovies = mostAdded,
                TopDirector = topDirector
            };
            return View(stats);
        }
    }
}
