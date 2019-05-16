namespace Vocalia.Podcast.DomainModels
{
    public class Subscription
    {
        public int ID { get; set; }
        public string UserUID { get; set; }
        public string RssUrl { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
