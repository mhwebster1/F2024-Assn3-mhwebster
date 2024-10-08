namespace F24_Assignment3_mwebster.Models
{
    public class ActorDetailsVM
    {
        public Actor actor { get; set; }
        public List<Movie> movies { get; set; }
        public ActorRedditAnalysisVM redditAnalysis { get; set; }
    }
}
