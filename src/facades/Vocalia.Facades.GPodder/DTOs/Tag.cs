using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Facades.GPodder.DTOs
{
    public class CategoryTag
    {
        /// <summary>
        /// Title of the category.
        /// </summary>
        [AliasAs("title")]
        public string Title { get; set; }

        /// <summary>
        /// Index for the category.
        /// </summary>
        [AliasAs("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Amount of podcasts the category belongs to.
        /// </summary>
        [AliasAs("usage")]
        public int Usage { get; set; }
    }
}
