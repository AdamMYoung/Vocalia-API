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
    public class ClipServiceBus : ObjectBus<IEnumerable<Clip>>
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

        public async override Task HandleMessageAsync(IEnumerable<Clip> messages)
        {
            using (var scope = ServiceScope.CreateScope())
            using (var DbContext = scope.ServiceProvider.GetService<EditorContext>())
            {
                foreach (var message in messages) { 
                    //Gets the specified session, or creates it if not available.
                    var session = await DbContext.Sessions.FirstOrDefaultAsync(x => x.UID == message.SessionUID);
                    if (session == null)
                    {
                        var sessionId = await CreateSessionAsync(message);
                        session = await DbContext.Sessions.FindAsync(sessionId);
                    }

                    //Gets the specificed user, or creates it if not available.
                    Db.User user = await DbContext.Users.FirstOrDefaultAsync(x => x.UserUID == message.UserUID && x.Session.UID == message.SessionUID);
                    if (user == null)
                    {
                        var userId = await CreateUserAsync(message, session.ID);
                        user = await DbContext.Users.FindAsync(userId);
                    }

                    //Adds the recieved media to the user entry.
                    DbContext.UserMedia.Add(new UserMedia
                    {
                        UserID = user.ID,
                        MediaUrl = message.MediaUrl,
                        Date = message.Date,
                        UID = message.UID
                    });
                }

                await DbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Creates a user in the database and returns the reference.
        /// </summary>
        /// <param name="recordingChunk">Recording chunk recieved.</param>
        /// <returns></returns>
        private async Task<int> CreateUserAsync(Clip recordingChunk, int sessionId)
        {
            using (var scope = ServiceScope.CreateScope())
            using (var DbContext = scope.ServiceProvider.GetService<EditorContext>())
            {
                var userRef = await UserFacade.GetUserInfoAsync(recordingChunk.UserUID);
                var user = new Db.User
                {
                    Name = userRef.name,
                    UserUID = recordingChunk.UserUID,
                    SessionID = sessionId
                };

                DbContext.Users.Add(user);
                await DbContext.SaveChangesAsync();

                return user.ID;
            }
        }

        /// <summary>
        /// Creates a session in the database and returns the reference.
        /// </summary>
        /// <param name="recordingChunk">Recording chunk recieved.</param>
        /// <returns></returns>
        private async Task<int> CreateSessionAsync(Clip recordingChunk)
        {
            using (var scope = ServiceScope.CreateScope())
            using (var DbContext = scope.ServiceProvider.GetService<EditorContext>())
            {
                var podcastRef = await DbContext.Podcasts.FirstOrDefaultAsync(x => x.UID == recordingChunk.PodcastUID);
                var session = new Session()
                {
                    IsFinishedEditing = false,
                    PodcastID = podcastRef.ID,
                    UID = recordingChunk.SessionUID,
                    Date = recordingChunk.Date
                };

                DbContext.Sessions.Add(session);
                await DbContext.SaveChangesAsync();

                return session.ID;
            }
        }
    }
}
