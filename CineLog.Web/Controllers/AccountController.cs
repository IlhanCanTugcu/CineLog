using CineLog.Core.Entities;
using CineLog.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CineLog.Web.Controllers
{
    public class AccountController : Controller
    {
        // Identity'nin kullanıcı yönetimi servisi
        private readonly UserManager<AppUser> _userManager;

        // Identity'nin giriş/çıkış yönetimi servisi (Login'de lazım olacak)
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // 1. Kayıt Sayfasını Getir (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // 2. Kayıt İşlemini Yap (POST)
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Form kurallarına uyulmuş mu? (Boş alan var mı vs.)
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // ViewModel'den -> Entity'ye (AppUser) dönüşüm
            var user = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName
            };

            // KAYIT İŞLEMİ (Şifreyi otomatik hashler/kriptolar)
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                TempData["Success"] = "Kayıt başarılı! Lütfen giriş yapın.";
                return RedirectToAction("Login"); // Login henüz yok ama yapacağız
            }
            else
            {
                // Hata varsa (Örn: Bu email zaten kayıtlı) ekrana bas
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        // 1. Giriş Sayfasını Getir (GET)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 2. Giriş İşlemini Yap (POST)
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Kullanıcıyı veritabanında bul (Kullanıcı adına göre)
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                return View(model);
            }

            // Şifre kontrolü ve Giriş Yapma (Cookie oluşturma)
            // false, false -> Beni hatırla yok, 5 kere yanlış girerse kilitleme yok.
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Movies"); // Ana sayfaya yönlendir
            }

            ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
            return View(model);
        }

        // 3. Çıkış Yap (Logout)
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}