using CineLog.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineLog.Data.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<UserMovie> UserMovies { get; set; }
        
        // AYARLAR BURADA YAPILIYOR
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Identity ayarlarını sakla (ÖNEMLİ!)

            // UserMovie tablosunun anahtarı (PK) tek değil, çifttir (UserId + MovieId)
            builder.Entity<UserMovie>()
                .HasKey(x => new { x.AppUserId, x.MovieId });

            // İlişkileri açıkça belirtelim (Opsiyonel ama garanti olur)
            builder.Entity<UserMovie>()
                .HasOne(x => x.Movie)
                .WithMany(x => x.UserMovies)
                .HasForeignKey(x => x.MovieId);

            builder.Entity<UserMovie>()
                .HasOne(x => x.AppUser)
                .WithMany(x => x.UserMovies)
                .HasForeignKey(x => x.AppUserId);
        }
    }
}
