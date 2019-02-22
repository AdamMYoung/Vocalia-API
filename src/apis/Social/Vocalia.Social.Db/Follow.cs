using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Social.Db
{
    public class Follow
    {
        public virtual int ID { get; set; }
        public virtual string UserUID { get; set; }
        public virtual string FollowUID { get; set; }

        public virtual User Follower { get; set; }
        public virtual User Following { get; set; }
    }
}
