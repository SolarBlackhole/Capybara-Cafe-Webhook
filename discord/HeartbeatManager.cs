using System.Timers;
using LabApi.Features.Wrappers;

namespace CapybaraCafePlugin.Discord
{
    public class HeartbeatManager {
        private static Timer _heartbeatTimer;

        public static int heartbeatInterval;
        public static void Start() {
            _heartbeatTimer = new Timer(heartbeatInterval * 1000);
            _heartbeatTimer.Elapsed += OnHeartbeat;
            _heartbeatTimer.AutoReset = true;
            _heartbeatTimer.Enabled = true;
        }

        public static void Stop() {
            if (_heartbeatTimer != null) {
                _heartbeatTimer.Stop();
                _heartbeatTimer.Dispose();
            }
        }

        private static void OnHeartbeat(object sender, ElapsedEventArgs e) {
            DiscordBridge.SendEvent("Heartbeat", false, new {
                Timestamp = System.DateTime.UtcNow,
                PlayerCount = Player.List.Count
            });
        }
    }
}