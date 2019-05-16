using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ObjectBus;
using ObjectBus.Options;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Publishing.Db;

namespace Vocalia.Publishing.ServiceBus
{
    public class PodcastServiceBus : ObjectBus<Vocalia.ServiceBus.Types.Publishing.Podcast>
    {
        private IServiceScopeFactory ServiceScope { get; }

        public PodcastServiceBus(IOptions<ObjectBusOptions<Vocalia.ServiceBus.Types.Publishing.Podcast>> options, IServiceScopeFactory serviceScope) : base(options)
        {
            ServiceScope = serviceScope;
        }

        public override async Task HandleMessageAsync(Vocalia.ServiceBus.Types.Publishing.Podcast message)
        {
            using (var scope = ServiceScope.CreateScope())
            using (var DbContext = scope.ServiceProvider.GetService<PublishContext>())
            {
                var podcast = new UnassignedPodcast
                {
                    Name = message.Name,
                    ImageUrl = message.ImageUrl,
                    UID = message.UID,
                    IsCompleted = false
                };
                DbContext.UnassignedPodcasts.Add(podcast);

                var members = message.Members.Select(c => new UnassignedPodcastMember
                {
                    UnassignedPodcastID = podcast.ID,
                    UserUID = c.UserUID
                });
                DbContext.UnassignedPodcastMembers.AddRange(members);

                await DbContext.SaveChangesAsync();
            }
        }
    }
}
