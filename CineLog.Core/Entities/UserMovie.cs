using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineLog.Core.Entities
{
    public class UserMovie
    {
        // 1. KULLANICI AYAĞI
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        // 2. FİLM AYAĞI
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        // 3. EKSTRA BİLGİLER
        public DateTime AddedDate { get; set; } = DateTime.Now;
        public bool IsWatched { get; set; } = false; 
        public int? Rating { get; set; } // Kullanıcının vereceği puan (1-10)
    }
}