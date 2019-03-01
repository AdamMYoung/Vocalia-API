using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Social.DomainModels
{
    public class User
    {
        public string UserUID { get; set; }
        public string UserTag { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PictureUrl { get; set; }
    }
}
