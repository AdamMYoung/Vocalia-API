using Microsoft.Extensions.Options;
using ObjectBus;
using ObjectBus.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Editor.Db;
using Microsoft.Extensions.DependencyInjection;

namespace Vocalia.Editor.ServiceBus
{
    public class PodcastServiceBus : ObjectBus<Vocalia.ServiceBus.Types.Podcast>
    {
        private IServiceScopeFactory ServiceScope { get; }

        public PodcastServiceBus(IOptions<ObjectBusOptions> options, IServiceScopeFactory serviceScope) : base(options)
        {
            ServiceScope = serviceScope;
        }

        public async override Task HandleMessageAsync(Vocalia.ServiceBus.Types.Podcast message)
        {
            using (var scope = ServiceScope.CreateScope())
            using (var DbContext = scope.ServiceProvider.GetService<EditorContext>())
            {
                if (!DbContext.Podcasts.Any(x => x.UID == message.UID))
                {
                    var podcast = new Db.Podcast
                    {
                        Name = message.Name,
                        ImageUrl = message.ImageUrl,
                        UID = message.UID
                    };

                    DbContext.Podcasts.Add(podcast);

                    var members = message.Members.Select(x => new Member
                    {
                        PodcastID = podcast.ID,
                        IsAdmin = x.IsAdmin,
                        UserUID = x.UserUID
                    });

                    DbContext.Members.AddRange(members);

                    await DbContext.SaveChangesAsync();
                }
            }
        }
    }
}
