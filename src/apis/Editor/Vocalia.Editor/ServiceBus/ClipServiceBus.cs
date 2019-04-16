using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ObjectBus;
using ObjectBus.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocalia.Editor.Db;
using Vocalia.ServiceBus.Types;
using Vocalia.UserFacade;
using Microsoft.Extensions.DependencyInjection;

namespace Vocalia.Editor.ServiceBus
{
    public class ClipServiceBus : ObjectBus<Vocalia.ServiceBus.Types.Clip>
    {
        /// <summary>
        /// Facade for handling Auth0 user info.
        /// </summary>
        private IUserFacade UserFacade { get; }

        private IServiceScopeFactory ServiceScope { get; }

        public ClipServiceBus(IOptions<ObjectBusOptions> options, IServiceScopeFactory serviceScope,
            IUserFacade userFacade) : base(options)
        {
            UserFacade = userFacade;
            ServiceScope = serviceScope;
        }

        public async override Task HandleMessageAsync(Vocalia.ServiceBus.Types.Clip message)
        {
            using (var scope = ServiceScope.CreateScope())
            using (var DbContext = scope.ServiceProvider.GetService<EditorContext>())
            {
                //Gets the specified session, or creates it if not available.
                var session = await DbContext.Sessions.FirstOrDefaultAsync(x => x.UID == message.SessionUID);
                if (session == null)
                {
                    var sessionId = await CreateSessionAsync(message);
                    session = await DbContext.Sessions.FindAsync(sessionId);
                }

                //Gets the specificed user, or creates it if not available.
                Db.Clip clip = await DbContext.Clips.FirstOrDefaultAsync(x => x.UID == message.UID && x.Session.UID == message.SessionUID);
                if (clip == null)
                {
                    var userId = await CreateClipAsync(message, session.ID);
                    clip = await DbContext.Clips.FindAsync(userId);
                }

                foreach (var entry in clip.Media)
                {
                    var userRef = await UserFacade.GetUserInfoAsync(entry.UserUID);
                    var media = new Db.Media
                    {
                        UID = entry.UID,
                        ClipID = clip.ID,
                        Date = entry.Date,
                        MediaUrl = entry.MediaUrl,
                        UserUID = entry.UserUID
                    };
                    DbContext.Media.Add(media);
                }

                await DbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Creates a user in the database and returns the reference.
        /// </summary>
        /// <param name="clip">Clip recieved.</param>
        /// <returns></returns>
        private async Task<int> CreateClipAsync(Vocalia.ServiceBus.Types.Clip clip, int sessionId)
        {
            using (var scope = ServiceScope.CreateScope())
            using (var DbContext = scope.ServiceProvider.GetService<EditorContext>())
            {
                var dbClip = new Db.Clip
                {
                    Name = clip.Name,
                    UID = clip.UID,
                    SessionID = sessionId,
                    Date = clip.Date
                };

                DbContext.Clips.Add(dbClip);              
                await DbContext.SaveChangesAsync();

                return dbClip.ID;
            }
        }

        /// <summary>
        /// Creates a session in the database and returns the reference.
        /// </summary>
        /// <param name="clip">Clip recieved.</param>
        /// <returns></returns>
        private async Task<int> CreateSessionAsync(Vocalia.ServiceBus.Types.Clip clip)
        {
            using (var scope = ServiceScope.CreateScope())
            using (var DbContext = scope.ServiceProvider.GetService<EditorContext>())
            {
                var podcastRef = await DbContext.Podcasts.FirstOrDefaultAsync(x => x.UID == clip.PodcastUID);
                var session = new Session()
                {
                    IsFinishedEditing = false,
                    PodcastID = podcastRef.ID,
                    UID = clip.SessionUID,
                    Date = clip.Date
                };

                DbContext.Sessions.Add(session);
                await DbContext.SaveChangesAsync();

                return session.ID;
            }
        }
    }
}
