using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Social.DomainModels
{
    public class User
    {
        public string user_id { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string nickname { get; set; }
        public string picture { get; set; }
    }
}
