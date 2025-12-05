namespace CineLog.Web.ViewModels
{
    public class StatsViewModel
    {
        public int TotalMovies { get; set; }
        public string FavoriteGenre { get; set; }
        public double AverageImdb { get; set; }
        public int TotalWatched { get; set; }
        public string TopDirector { get; set; }
        public List<PopularMovieDto> MostAddedMovies { get; set; }
    }

    public class PopularMovieDto
    {
        public string Title { get; set; }
        public string PosterUrl { get; set; }
        public string ImdbRating { get; set; }
        public int AddCount { get; set; } 
    }
}
