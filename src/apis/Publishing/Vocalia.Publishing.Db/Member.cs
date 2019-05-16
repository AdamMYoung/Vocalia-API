namespace Vocalia.Publishing.Db
{
    public class Member
    {
        public virtual int ID { get; set; }
        public virtual int PodcastID { get; set; }
        public virtual string UserUID { get; set; }

        public virtual Podcast Podcast { get; set; }
    }
}
