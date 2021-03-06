﻿namespace Vocalia.Editor.Db
{
    public class Member
    {
        public virtual int ID { get; set; }
        public virtual int PodcastID { get; set; }
        public virtual string UserUID { get; set; }
        public virtual bool IsAdmin { get; set; }

        public virtual Podcast Podcast { get; set; }

    }
}
