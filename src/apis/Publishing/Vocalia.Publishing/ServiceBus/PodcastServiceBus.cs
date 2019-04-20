using Microsoft.Extensions.Options;
using ObjectBus;
using ObjectBus.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.ServiceBus.Types;

namespace Vocalia.Publishing.ServiceBus
{
    public class PodcastServiceBus : ObjectBus<Podcast>
    {
        public PodcastServiceBus(IOptions<ObjectBusOptions> options) : base(options) { }

        public override Task HandleMessageAsync(Podcast message)
        {
            return base.HandleMessageAsync(message);
        }
    }
}
