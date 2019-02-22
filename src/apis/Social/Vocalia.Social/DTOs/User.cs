using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Social.DTOs
{
    public class User
    {
        public string UserUID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public IEnumerable<Listen> Feed { get; set; }
    }
}
