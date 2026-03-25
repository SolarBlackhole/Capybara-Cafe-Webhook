using System.Linq;
using LabApi.Events.Arguments.ObjectiveEvents;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp0492Events;
using LabApi.Events.Arguments.Scp049Events;
using LabApi.Events.Arguments.Scp079Events;
using LabApi.Events.Arguments.Scp096Events;
using LabApi.Events.Arguments.Scp106Events;
using LabApi.Events.Arguments.Scp127Events;
using LabApi.Events.Arguments.Scp173Events;
using LabApi.Events.Arguments.Scp3114Events;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Events.Arguments.Scp939Events;
using LabApi.Events.Arguments.ScpEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;

namespace Capybara_Cafe_Webhook;
public class EventHandler : CapyBaraEventHandler
{
    private float roundStartTime;

    /*
    * Events to be sent to both API and Other Files
    */
    public void OnRoundStart()
    {
        roundStartTime = UnityEngine.Time.time;
        Log.Info("Round started.");

        // Send info to server API
        // Will need to send Player count as well
    }

    [PluginEvent(ServerEventType.PlayerEscape)]
    public void OnPlayerEscape(Player player, RoleTypeId role)
    {
        float time = UnityEngine.Time.time - roundStartTime;
        Log.Info($"Player {player.Name} escaped in {time}s.");

        // Send info to server API and leaderboard
    }

    public override void OnPlayerJoined(PlayerJoinedEventArgs ev)
    {
        ev.Player.SendBroadcast("Welcome to the Capybara Cafe!", 10);
        // Send info to server API
    }

    [PluginEvent(ServerEventType.PlayerLeft)]
    public void OnPlayerLeft(Player player)
    {
        // Send info to server API
    }

    /*
    * Events to be sent to only API
    */

}
