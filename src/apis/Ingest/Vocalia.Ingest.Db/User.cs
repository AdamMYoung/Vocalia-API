﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Ingest.Db
{
    public class User
    {
        public virtual string UserUID { get; set; }
        public virtual string UserTag { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual bool Active { get; set; }

        public virtual IEnumerable<UserGroup> UserGroups { get; set; }
        
    }
}