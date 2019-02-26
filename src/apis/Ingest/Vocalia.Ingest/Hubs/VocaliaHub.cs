﻿using Microsoft.AspNetCore.SignalR;
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
        private static List<DomainModels.User> Users { get; } = new List<DomainModels.User>();

        /// <summary>
        /// Assigns the connecting user to the specified group.
        /// </summary>
        /// <param name="groupId">GUID of the group.</param>
        /// <returns></returns>
        public async Task JoinGroup(string userTag, string groupId)
        {
            DomainModels.User user = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (user != null)
            {
                await Groups.RemoveFromGroupAsync(user.ConnectionId, user.CurrentGroupId);
                Users.RemoveAll(x => x.ConnectionId == Context.ConnectionId);
            }

            Users.Add(new DomainModels.User
            {
                UserTag = userTag,
                CurrentGroupId = groupId,
                ConnectionId = Context.ConnectionId
            });

            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }

        /// <summary>
        /// Returns all group members aside from the calling user belonging to the current group.
        /// </summary>
        /// <returns></returns>
        public async Task QueryGroupMembers()
        {
            var user = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            var users = Users.Where(x => x.CurrentGroupId == user.CurrentGroupId).Where(x => x.ConnectionId != Context.ConnectionId);

            await Clients.Client(user.ConnectionId)
                .SendAsync("onMembersAcquired", users.Select(x => x.ConnectionId));
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
                await Clients.Client(targetId).SendAsync("onOffer", offer, sender.ConnectionId);
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
                .SendAsync("onAnswer", answer, sender.ConnectionId);
        }

        /// <summary>
        /// Called when a new ICE candidate is recieved.
        /// </summary>
        /// <param name="data">Data to transmit.</param>
        /// <returns></returns>
        public async Task NewCandidate(string candidate, string key)
        {
            var user = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (user != null)
                await Clients.Client(key).SendAsync("onCandidate", candidate, user.ConnectionId);
        }
    }
}
