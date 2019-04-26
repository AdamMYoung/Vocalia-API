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
    public class PodcastServiceBus : ObjectBus<Vocalia.ServiceBus.Types.Editor.Podcast>
    {
        private IServiceScopeFactory ServiceScope { get; }

        public PodcastServiceBus(IOptions<ObjectBusOptions> options, IServiceScopeFactory serviceScope) : base(options)
        {
            ServiceScope = serviceScope;
        }

        public async override Task HandleMessageAsync(Vocalia.ServiceBus.Types.Editor.Podcast message)
        {
            using (var scope = ServiceScope.CreateScope())
            using (var DbContext = scope.ServiceProvider.GetService<EditorContext>())
            {
                //Pass onto next service bus.
                var publishServiceBus = scope.ServiceProvider.GetService<IObjectBus<Vocalia.ServiceBus.Types.Publishing.Podcast>>();
                await publishServiceBus.SendAsync(new Vocalia.ServiceBus.Types.Publishing.Podcast
                {
                    UID = message.UID,
                    ImageUrl = message.ImageUrl,
                    Name = message.Name,
                    Members = message.Members
                });

                //Update database of editor.
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
