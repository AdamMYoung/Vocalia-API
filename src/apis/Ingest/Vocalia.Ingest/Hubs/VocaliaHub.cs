using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Vocalia.Ingest.Hubs
{
    public class VocaliaHub : Hub
    {
        /// <summary>
        /// Collection of current users in a call.
        /// </summary>
        public static List<DomainModels.SignalRUser> Users { get; } = new List<DomainModels.SignalRUser>();

        /// <summary>
        /// Collection of active sessions.
        /// </summary>
        public static List<DomainModels.SignalRSession> Sessions { get; } = new List<DomainModels.SignalRSession>();

        private IHubContext<VocaliaHub> HubContext { get; }

        public VocaliaHub(IHubContext<VocaliaHub> hubContext)
        {
            HubContext = hubContext;
        }

        /// <summary>
        /// Assigns the connecting user to the specified group.
        /// </summary>
        /// <param name="groupId">GUID of the group.</param>
        /// <returns></returns>
        public async Task JoinGroup(string userTag, string sessionId)
        {
            DomainModels.SignalRUser user = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (user != null)
            {
                await Groups.RemoveFromGroupAsync(user.ConnectionId, user.CurrentSessionId);
                Users.RemoveAll(x => x.ConnectionId == Context.ConnectionId);
            }

            Users.Add(new DomainModels.SignalRUser
            {
                UserTag = userTag,
                CurrentSessionId = sessionId,
                ConnectionId = Context.ConnectionId
            });

            if (!Sessions.Any(x => x.SessionUID == sessionId))
            {
          
                Sessions.Add(new DomainModels.SignalRSession(HubContext)
                {
                    SessionUID = sessionId
                });

            }
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
            await PushInitialSessionInfo();
        }

        /// <summary>
        /// Returns all group members aside from the calling user belonging to the current group.
        /// </summary>
        /// <returns></returns>
        public async Task QueryGroupMembers()
        {
            var user = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            var users = Users.Where(x => x.CurrentSessionId == user.CurrentSessionId).Where(x => x.ConnectionId != Context.ConnectionId);

            await Clients.Client(user.ConnectionId)
                .SendAsync("onMembersAcquired", users.Select(x => 
                new DTOs.SignalRUser { ID = x.ConnectionId, Tag = x.UserTag }));
        }

        /// <summary>
        /// Removes the connected user from the specified group.
        /// </summary>
        /// <param name="groupId">GUID of the group.</param>
        /// <returns></returns>
        public async Task LeaveGroup(string groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
            Users.RemoveAll(x => x.ConnectionId == Context.ConnectionId);
        }

        /// <summary>
        /// Sends the SDP offer to other members in the group.
        /// </summary>
        /// <param name="data">Data to transmit.</param>
        /// <returns></returns>
        public async Task SendOffer(string offer, string targetId)
        {
            var sender = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (sender != null)
                await Clients.Client(targetId).SendAsync("onOffer", offer, 
                    new DTOs.SignalRUser { ID = sender.ConnectionId, Tag = sender.UserTag});
        }

        /// <summary>
        /// Sends the SDP answer to other members in the group.
        /// </summary>
        /// <param name="data">Data to transmit.</param>
        /// <returns></returns>
        public async Task SendAnswer(string answer, string targetId)
        {
            var sender = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            await Clients.Client(targetId)
                .SendAsync("onAnswer", answer,
                new DTOs.SignalRUser { ID = sender.ConnectionId, Tag = sender.UserTag });
        }

        /// <summary>
        /// Called when a new ICE candidate is recieved.
        /// </summary>
        /// <param name="data">Data to transmit.</param>
        /// <returns></returns>
        public async Task NewCandidate(string candidate, string targetId)
        {
            var user = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (user != null)
                await Clients.Client(targetId).SendAsync("onCandidate", candidate, user.ConnectionId);
        }

        /// <summary>
        /// Called when the recoding status has changed in a session.
        /// </summary>
        /// <param name="isRecording">Recording status.</param>
        /// <returns></returns>
        public async Task SetRecording(bool isRecording)
        {
            var user = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            var session = Sessions.FirstOrDefault(x => x.SessionUID == user.CurrentSessionId);

            var sessionUsers = Users.Where(x => x.CurrentSessionId == session.SessionUID);

            session.IsRecording = isRecording;
            var clients = Clients.Clients(sessionUsers.Select(c => c.ConnectionId).ToList());
            await clients.SendAsync("onRecordingChanged", isRecording);
        }

        /// <summary>
        /// Called when the paused status has changed in a session.
        /// </summary>
        /// <param name="isPaused">Paused status.</param>
        /// <returns></returns>
        public async Task SetPaused(bool isPaused)
        {
            var user = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            var session = Sessions.FirstOrDefault(x => x.SessionUID == user.CurrentSessionId);

            var sessionUsers = Users.Where(x => x.CurrentSessionId == session.SessionUID);

            session.IsPaused = isPaused;
            await Clients.Clients(sessionUsers.Select(c => c.ConnectionId).ToList())
                .SendAsync("onPauseChanged", isPaused);
        }

        /// <summary>
        /// Updates the joining client with the current session information.
        /// </summary>
        /// <returns></returns>
        private async Task PushInitialSessionInfo()
        {
            var user = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            var session = Sessions.FirstOrDefault(x => x.SessionUID == user.CurrentSessionId);

            await Clients.Client(user.ConnectionId).SendAsync("onPauseChanged", session.IsPaused);
            await Clients.Client(user.ConnectionId).SendAsync("onRecordingChanged", session.IsRecording);
        }
    }
}
