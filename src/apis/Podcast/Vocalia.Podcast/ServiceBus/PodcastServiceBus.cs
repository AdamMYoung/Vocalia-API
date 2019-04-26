using Microsoft.Extensions.Options;
using ObjectBus;
using ObjectBus.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Vocalia.Podcast.Db;

namespace Vocalia.Podcast.ServiceBus
{
    public class PodcastServiceBus : ObjectBus<Vocalia.ServiceBus.Types.Podcast.Podcast>
    {
        private IServiceScopeFactory ServiceScope { get; }

        public PodcastServiceBus(IOptions<ObjectBusOptions<Vocalia.ServiceBus.Types.Podcast.Podcast>> options, IServiceScopeFactory serviceScope) : base(options)
        {
            ServiceScope = serviceScope;
        }

        public async override Task HandleMessageAsync(Vocalia.ServiceBus.Types.Podcast.Podcast message)
        {
            using (var scope = ServiceScope.CreateScope())
            using (var DbContext = scope.ServiceProvider.GetService<PodcastContext>())
            {
                //Update database of editor.
                if (!DbContext.Podcasts.Any(x => x.UID == message.UID))
                {
                    var podcast = new Db.Podcast
                    {
                        Title = message.Title,
                        ImageUrl = message.ImageUrl,
                        RSS = message.RssUrl,
                        IsExplicit = message.IsExplicit,
                        CategoryID = message.CategoryID,
                        LanguageID = message.LanguageID,
                        Active = true,
                        UID = message.UID
                    };
                    DbContext.Podcasts.Add(podcast);

                    await DbContext.SaveChangesAsync();
                }
            }
        }
    }
}
