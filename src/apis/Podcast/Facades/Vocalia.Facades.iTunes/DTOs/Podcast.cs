namespace Vocalia.Facades.iTunes.DTOs
{
    public class Podcast
    {
        public int Position { get; internal set; }

        public int PodcastId { get; set; }

        public string Name { get; set; }

        public string ArtistName { get; set; }

        public string ImageUrl { get; set; }

        public string RssUrl { get; set; }

        
    }
}
