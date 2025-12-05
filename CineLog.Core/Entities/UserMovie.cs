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

        // 3. EKSTRA BİLGİLER (Payload)
        public DateTime AddedDate { get; set; } = DateTime.Now;
        public bool IsWatched { get; set; } = false; // "İzledim" işaretlemek için
        public int? Rating { get; set; } // Kullanıcının vereceği özel puan (1-10)
    }
}