namespace F24_Assignment3_mwebster.Models
{
    public class MovieDetailsVM
    {
        public Movie movie { get; set; }
        public List<Actor> actors { get; set; }
        public MovieRedditAnalysisVM redditAnalysis { get; set; }
    }
}
