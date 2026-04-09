using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;


namespace CapybaraCafePlugin.EventListeners
{
    public class SCPEventListener : CustomEventsHandler
    {
        public override void OnPlayerJoined(PlayerJoinedEventArgs ev)
        {
            string message = "Welcome to the Capybara Cafe!\n Please read the <color=#e74c3c>rules</color> and check out our <color=#7289da>Discord</color>. Have Fun!";
            ev.Player.SendBroadcast(message, 10);

            // Send info to server API
        }
    }
}
