using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ObjectBus;
using ObjectBus.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Publishing.Db;
using Vocalia.ServiceBus.Types.Publishing;

namespace Vocalia.Publishing.ServiceBus
{
    public class TimelineServiceBus : ObjectBus<Timeline>
    {
        private IServiceScopeFactory ServiceScope { get; }

        public TimelineServiceBus(IOptions<ObjectBusOptions> options, IServiceScopeFactory serviceScope) : base(options)
        {
            ServiceScope = serviceScope;
        }

        public override async Task HandleMessageAsync(Timeline message)
        {
            using (var scope = ServiceScope.CreateScope())
            using (var DbContext = scope.ServiceProvider.GetService<PublishContext>())
            {
                var podcast = DbContext.UnassignedPodcasts.FirstOrDefault(c => c.UID == message.PodcastUID);

                if (podcast != null)
                {
                    var episode = new UnassignedEpisode
                    {
                        UID = message.UID,
                        Name = message.Name,
                        IsCompleted = false,
                        UnassignedPodcastID = podcast.ID
                    };
                    DbContext.UnassignedEpisodes.Add(episode);

                    var clips = message.TimelineEntries.Select(c => new UnassignedEpisodeClip
                    {
                        MediaUrl = c,
                        UnassignedEpisodeID = episode.ID,
                        Position = message.TimelineEntries.IndexOf(c)
                    });
                    DbContext.UnassignedEpisodeClips.AddRange(clips);

                    await DbContext.SaveChangesAsync();
                }
            }
        }
    }
}
