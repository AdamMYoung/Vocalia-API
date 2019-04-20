using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Publishing.Db
{
    public class PublishContext : DbContext
    {
        public PublishContext(DbContextOptions<PublishContext> options) : base(options) { }
    }
}
