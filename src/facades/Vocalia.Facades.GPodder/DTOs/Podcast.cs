using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Facades.GPodder.DTOs
{
    public class Podcast
    {
        /// <summary>
        /// URL of the podcast website.
        /// </summary>
        [AliasAs("url")]
        public string URL { get; set; }

        /// <summary>
        /// Title of the podcast
        /// </summary>
        [AliasAs("title")]
        public string Title { get; set; }

        /// <summary>
        /// Descripton of the podcast
        /// </summary>
        [AliasAs("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Number of subscribers the podcast has.
        /// </summary>
        [AliasAs("subscribers")]
        public int Subscribers { get; set; }

        /// <summary>
        /// Logo of the podcast.
        /// </summary>
        [AliasAs("logo_url")]
        public string LogoURL { get; set; }

        /// <summary>
        /// Website of the podcast author.
        /// </summary>
        [AliasAs("website")]
        public string Website { get; set; }

        /// <summary>
        /// Link to the GPodder podcast entry.
        /// </summary>
        [AliasAs("mygpo_link")]
        public string MyGPOLink { get; set; }
    }
}
