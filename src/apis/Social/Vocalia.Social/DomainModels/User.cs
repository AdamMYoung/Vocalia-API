using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Social.DomainModels
{
    public class User
    {
        public string UserUID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public bool Active { get; set; }
    }
}
