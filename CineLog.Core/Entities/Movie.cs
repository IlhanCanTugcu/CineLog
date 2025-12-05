using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineLog.Core.Entities
{
    public class Movie : BaseEntity
    {
        public string Title { get; set; } = null!;       // Film Adı
        public string? Description { get; set; }         // Konusu 
        public string? Director { get; set; }            // Yönetmen
        public string? Year { get; set; }                // Yapım Yılı
        public string? PosterUrl { get; set; }           // Afiş Resmi Linki
        public string? ImdbId { get; set; } = null!;     // Örn: tt1375666 
        public string? Genre { get; set; }               // Tür 
        public string? ImdbRating { get; set; }          // Puan 
        public ICollection<UserMovie> UserMovies { get; set; }    // Bir film, birden çok kullanıcının listesinde olabilir

    }
}
