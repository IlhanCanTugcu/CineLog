using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CineLog.Core.DTOs
{
    public class OmdbSearchResponse
    {
        [JsonPropertyName("Search")]
        public List<OmdbSearchResult>? Search { get; set; } // Film Listesi

        [JsonPropertyName("totalResults")]
        public string? TotalResults { get; set; }

        [JsonPropertyName("Response")]
        public string? Response { get; set; }
    }

    // Listedeki her bir filmin özeti (Detaylar yok, sadece vitrinlik bilgiler)
    public class OmdbSearchResult
    {
        [JsonPropertyName("Title")]
        public string? Title { get; set; }

        [JsonPropertyName("Year")]
        public string? Year { get; set; }

        [JsonPropertyName("imdbID")]
        public string? ImdbId { get; set; }

        [JsonPropertyName("Type")]
        public string? Type { get; set; }

        [JsonPropertyName("Poster")]
        public string? PosterUrl { get; set; }
    }
}
