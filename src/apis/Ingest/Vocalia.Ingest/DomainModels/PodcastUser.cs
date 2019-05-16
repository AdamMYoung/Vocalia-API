namespace Vocalia.Ingest.DomainModels
{
    public class PodcastUser
    {
        public int ID { get; set; }
        public int PodcastID { get; set; }
        public string UID { get; set; }
        public bool IsAdmin { get; set; }
    }
}
