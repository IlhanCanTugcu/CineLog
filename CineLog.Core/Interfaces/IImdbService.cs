using CineLog.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineLog.Core.Interfaces
{
    public interface IImdbService
    {
        Task<OmdbDto?> GetMovieByIdAsync(string imdbId);
        Task<OmdbSearchResponse?> SearchMoviesAsync(string searchTerm);
    }
}
