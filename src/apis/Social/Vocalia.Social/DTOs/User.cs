using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Social.DTOs
{
    public class User
    {
        public string UserTag { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PictureUrl { get; set; }
        public IEnumerable<Listen> Feed { get; set; } = new List<Listen>();
        public IEnumerable<User> Following { get; set; } = new List<User>();
        public IEnumerable<User> Followers { get; set; } = new List<User>();
    }
}
