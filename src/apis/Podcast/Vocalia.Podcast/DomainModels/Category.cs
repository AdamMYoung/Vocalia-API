using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vocalia.Podcast.DomainModels
{
    public class Category
    {
        /// <summary>
        /// ID of the category.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ITunes API ID reference of the category.
        /// </summary>
        public int ITunesID { get; set; }

        /// <summary>
        /// GPodder API Tag reference of the category.
        /// </summary>
        public string GPodderTag { get; set; }

        /// <summary>
        /// Title of the category.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Icon URL of the category.
        /// </summary>
        public string IconUrl { get; set; }
    }
}
