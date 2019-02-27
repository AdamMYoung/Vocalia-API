using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Ingest.DomainModels;

namespace Vocalia.Ingest.Repositories
{
    public class IngestRepository : IIngestRepository
    {
        public Task<IEnumerable<Podcast>> GetGroupPodcastsAsync(string groupUID)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Group>> GetUserGroupsAsync(string userUID)
        {
            throw new NotImplementedException();
        }
    }
}
