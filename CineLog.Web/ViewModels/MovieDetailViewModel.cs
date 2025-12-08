using CineLog.Core.DTOs;
using CineLog.Core.Entities;
using System.Reflection.Metadata.Ecma335;

namespace CineLog.Web.ViewModels
{
    public class MovieDetailViewModel
    {
        public OmdbDto Details { get; set; }
        public List<Movie> Recommendations { get; set; }
    }
}
