using Microsoft.Extensions.Options;
using ObjectBus;
using ObjectBus.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.ServiceBus.Types.Publishing;

namespace Vocalia.Publishing.ServiceBus
{
    public class TimelineServiceBus : ObjectBus<Timeline>
    {
        public TimelineServiceBus(IOptions<ObjectBusOptions> options) : base(options) { }

        public override Task HandleMessageAsync(Timeline message)
        {
            return base.HandleMessageAsync(message);
        }
    }
}
