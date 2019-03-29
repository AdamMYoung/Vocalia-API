using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Vocalia.Ingest.Hubs;
using Microsoft.AspNetCore.SignalR.Client;

namespace Vocalia.Ingest.DomainModels
{
    public class SignalRSession
    {
        public string SessionUID { get; set; }
        public int Duration { get; set; } = 0;
        private bool _isPaused { get; set; } = false;
        public bool IsPaused 
        {
            get {
                return _isPaused;
            }
            set {
                _isPaused = value;
                OnPausedChanged(value);
            }
        }
        private bool _isRecording { get; set; } = false;
        public bool IsRecording
        {
            get 
            {
                return _isRecording;
            }
            set
            {
                _isRecording = value;
                OnRecordingChanged(value);
            }
        }
        public Timer SessionTimer { get; } = new Timer(1000);
        private IHubContext<VocaliaHub> HubContext { get; }

        public SignalRSession(IHubContext<VocaliaHub> hub)
        {
            HubContext = hub;
            SessionTimer.Elapsed += SessionTimer_Elapsed;
        }

        private void SessionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {          
            Task.Run(async () =>
            {
                Duration++;
                var sessionUsers = VocaliaHub.Users.Where(x => x.CurrentSessionId == SessionUID);
                await HubContext.Clients.Clients(sessionUsers.Select(c => c.ConnectionId).ToList())
             .SendAsync("onTimeChanged", Duration);
            });
        }

        /// <summary>
        /// Called when paused status changes.
        /// </summary>
        /// <param name="isPaused">Paused status.</param>
        private void OnPausedChanged(bool isPaused)
        {
            if (isPaused)   
                SessionTimer.Stop();
            else          
                SessionTimer.Start();
        }

        /// <summary>
        /// Called when the recording status changes.
        /// </summary>
        /// <param name="isRecording">Recording status.</param>
        private void OnRecordingChanged(bool isRecording)
        {
            if (!isRecording)
            {
                SessionTimer.Stop();
                Duration = 0;
            }
            else
            {
                SessionTimer.Start();
            }
        }
    }
}
