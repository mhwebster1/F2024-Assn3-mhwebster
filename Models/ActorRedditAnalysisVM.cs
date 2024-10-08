namespace F24_Assignment3_mwebster.Models
{
    public class ActorRedditAnalysisVM
    {
        public string ActorName { get; set; }
        public List<RedditSentiment> RedditSentiments { get; set; }
        public double OverallSentiment { get; set; }
    }
    public class MovieRedditAnalysisVM
    {
        public string MovieTitle { get; set; }
        public List<RedditSentiment> RedditSentiments { get; set; }
        public double OverallSentiment { get; set; }
    }
    public class RedditSentiment
    {
        public string RedditText { get; set; }
        public double SentimentScore {  get; set; }
    }
}