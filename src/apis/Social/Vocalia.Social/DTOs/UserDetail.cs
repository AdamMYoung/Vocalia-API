using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Social.DTOs
{
    public class UserDetail
    {
        public string UserUID { get; set; }
        public string UserTag { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PictureUrl { get; set; }
        public IEnumerable<Listen> Feed { get; set; } = new List<Listen>();
        public IEnumerable<UserDetail> Following { get; set; } = new List<UserDetail>();
        public IEnumerable<UserDetail> Followers { get; set; } = new List<UserDetail>();
    }
}
