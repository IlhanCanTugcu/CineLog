using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CineLog.Core.DTOs
{
    public class OmdbDto
    {
        [JsonPropertyName("Title")]
        public string? Title { get; set; }

        [JsonPropertyName("Year")]
        public string? Year { get; set; }

        [JsonPropertyName("Genre")]
        public string? Genre { get; set; }

        [JsonPropertyName("Director")]
        public string? Director { get; set; }

        [JsonPropertyName("Plot")]
        public string? Description { get; set; } 

        [JsonPropertyName("Poster")]
        public string? PosterUrl { get; set; } // Bizim entity'de 'PosterUrl' olacak

        [JsonPropertyName("imdbRating")]
        public string? ImdbRating { get; set; } // Dikkat: API'de küçük harfle başlıyor

        [JsonPropertyName("imdbID")]
        public string? ImdbId { get; set; }

        [JsonPropertyName("Response")]
        public string? Response { get; set; } // "True" veya "False" döner

        [JsonPropertyName("Error")]
        public string? Error { get; set; } // Film bulunamazsa hata mesajı buraya gelir
    }
}
