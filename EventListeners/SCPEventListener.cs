using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using PlayerRoles;
using RemoteAdmin;
using CapybaraCafePlugin.Discord;
using PlayerStatsSystem;
using LabApi.Features.Console;
using PlayerRoles.PlayableScps.Scp939;
using PlayerRoles.PlayableScps.Scp3114;
using InventorySystem.Items.Scp1509;
using PlayerRoles.PlayableScps.Scp1507;


namespace CapybaraCafePlugin.EventListeners {
    public class SCPEventListener : CustomEventsHandler {

        private Player fastestEscapePlayer;
        private TimeSpan escapeTime;
        private RoleTypeId escapeRole;
        // Player events
        public override void OnPlayerJoined(PlayerJoinedEventArgs ev) {
            string message = "Welcome to the Capybara Cafe!\n Please read the <color=#e74c3c>rules</color> and check out our <color=#7289da>Discord</color>. Have Fun!";
            ev.Player.SendBroadcast(message, 10);

            DiscordBridge.SendEvent("PlayerJoined", false, new {
                PlayerName = ev.Player.Nickname,
                PlayerId = ev.Player.UserId,
                PlayerCount = Player.List.Count
            });

            // Send info to server API
        }
        public override void OnPlayerLeft(PlayerLeftEventArgs ev) {
            // Send info to server API
            DiscordBridge.SendEvent("PlayerLeft", false, new {
                PlayerName = ev.Player.Nickname,
                PlayerId = ev.Player.UserId,
                PlayerCount = Player.List.Count - 1
            });
        }

        public override void OnPlayerDeath(PlayerDeathEventArgs ev) {
            // Send info to server API
            if (ev.Player == null || ev.Player.Role == RoleTypeId.None)
            {
                return;
            }
            if (ev.Attacker == null || ev.Player.PlayerId == ev.Attacker.PlayerId)
            {
                DiscordBridge.SendEvent("PlayerDied", false, new
                {
                    PlayerName = ev.Player.Nickname,
                    PlayerId = ev.Player.UserId,
                    Role = ev.OldRole.ToString(),
                    DamageType = GetDamageType(ev.DamageHandler)
                });
                return;
            }
            else
            {
                DiscordBridge.SendEvent("PlayerKilled", false, new
                {
                    VictimName = ev.Player.Nickname,
                    VictimId = ev.Player.UserId,
                    VictimRole = ev.OldRole.ToString(),
                    AttackerName = ev.Attacker.Nickname,
                    AttackerId = ev.Attacker.UserId,
                    AttackerRole = ev.Attacker.Role.ToString(),
                    DamageType = GetDamageType(ev.DamageHandler) // Will Address Later
                });
            }
        }
        public override void OnServerWaveRespawned(WaveRespawnedEventArgs ev) {
            // Send info to server API
            DiscordBridge.SendEvent("ServerWaveRespawned", false, new {
                PlayersRespawned = ev.Players.Select(p => p.Nickname).ToArray(),
                Faction = ev.Wave.Faction == Faction.FoundationEnemy ? "Chaos" : "MTF"
            });
        }
        public override void OnPlayerEscaped(PlayerEscapedEventArgs ev) {
            // Send info to server API
            if (ev.Player == null || ev.Player.Role == RoleTypeId.None)
            {
                return;
            }
            if (fastestEscapePlayer == null || Round.Duration < escapeTime)
            {
                fastestEscapePlayer = ev.Player;
                escapeTime = Round.Duration;
                escapeRole = ev.OldRole;
            }
            DiscordBridge.SendEvent("PlayerEscaped", false, new {
                PlayerName = ev.Player.Nickname,
                PlayerId = ev.Player.UserId,
                Role = ev.OldRole.ToString()
            });
        }
        public override void OnServerSentAdminChat(SentAdminChatEventArgs ev) {
            string message = ev.Message.TrimStart('@');
            if (string.IsNullOrEmpty(message)) return;

            // Cast ev.Sender to a player-specific sender to get the hub
            var sender = ev.Sender as CommandSender;
            Player player = null;

            if (sender is PlayerCommandSender playerSender)
            {
                player = Player.Get(playerSender.ReferenceHub);
            }

            DiscordBridge.SendEvent("AdminChatMessage", false, new {
                SenderName = ev.Sender.Nickname,
                SenderId = player != null ? player.UserId : "Server",
                Message = message
            });
        }
        // Server events
        public override void OnServerRoundStarted() {
            // Send info to server API
            fastestEscapePlayer = null;
            Server.FriendlyFire = false;
            DiscordBridge.SendEvent("ServerRoundStarted", false, new {
                PlayerCount = Player.List.Count,
                Players = Player.List.Select(p => new {
                    PlayerName = p.Nickname,
                    PlayerId = p.UserId,
                    Role = p.Role.ToString()
                }).ToArray()
            });
        }
        public override void OnServerRoundEnding(RoundEndingEventArgs ev) {
            // Friendly Fire toggle on
            string escapeText = "Null";
            bool anyEscape = fastestEscapePlayer != null ? true : false;
            if (anyEscape && escapeRole == RoleTypeId.ClassD)
            {
                escapeText = "<color=#ff8e00>Class-D</color>";
            } else if (anyEscape && escapeRole == RoleTypeId.Scientist)
            {
                escapeText = "<color=#ffff7c>Scientist</color>";
            }

            foreach (Player player in Player.GetAll())
            {
                player.SendHint("Friendly Fire has been enabled", 10);
                if (anyEscape)
                {
                    player.SendBroadcast("<color=#22EC22>" + fastestEscapePlayer.Nickname + "</color> escaped in " + escapeTime.ToString(@"mm\:ss") + " as a " + escapeText, 20);
                }
            }
            Server.FriendlyFire = true;
        }
        public override void OnServerRoundEnded(RoundEndedEventArgs ev) {            
            // Send info to server API
            DiscordBridge.SendEvent("ServerRoundEnded", false, new {
                WinningTeam = ev.LeadingTeam.ToString(),
                EscapedDClass = RoundSummary.EscapedClassD.ToString(),
                EscapedScientists = RoundSummary.EscapedScientists.ToString(),
                SCPKills = RoundSummary.KilledBySCPs.ToString(),
                SurvivingSCPs = RoundSummary.SurvivingSCPs.ToString(),
                WarheadDetonated = Warhead.IsDetonated.ToString()
            });
        }
        public override void OnServerWaitingForPlayers() {
            // Send info to server API
            Server.FriendlyFire = false;
            DiscordBridge.SendEvent("ServerWaitingForPlayers", false, null);
        }
        public override void OnServerCommandExecuted(CommandExecutedEventArgs ev)
        {
            var sender = ev.Sender as CommandSender;
            Player player = null;

            if (sender is PlayerCommandSender playerSender)
            {
                player = Player.Get(playerSender.ReferenceHub);
            }

            DiscordBridge.SendEvent("CommandExecuted", false, new
            {
               PlayerName = ev.Sender.Nickname,
               PlayerId = player != null ? player.UserId : "Server",
               CommandType = ev.CommandType,
               Command = ev.Command,
               Arguments = ev.Arguments,
               Success = ev.ExecutedSuccessfully,
               Response = ev.Response
            });
        }
        public override void OnPlayerCuffed(PlayerCuffedEventArgs ev)
        {
            DiscordBridge.SendEvent("PlayerCuffed", false, new
            {
                PlayerName = ev.Player.Nickname,
                PlayerId = ev.Player.UserId,
                TargetName = ev.Target.Nickname,
                TargetId = ev.Target.UserId
            });
        }

        // Punishment Events
        public override void OnPlayerBanned(PlayerBannedEventArgs ev) {
            // Send info to server API
            if (ev.Player == null) return;
            string iName = !string.IsNullOrEmpty(ev.Issuer.Nickname) ? ev.Issuer.Nickname : "Server";
            string iId = (ev.Issuer is Player issuerPlayer) ? issuerPlayer.UserId : "Server";
            if (ev.Duration == 0) {
                DiscordBridge.SendEvent("PlayerKicked", true, new {
                    PlayerName = ev.Player.Nickname,
                    PlayerId = ev.Player.UserId,
                    Reasoning = ev.Reason,
                    IssuerName = iName,
                    IssuerId = iId
                });
            } else {
                DiscordBridge.SendEvent("PlayerBanned", true, new {
                    PlayerName = ev.Player.Nickname,
                    PlayerId = ev.Player.UserId,
                    Reasoning = ev.Reason,
                    DurationSeconds = ev.Duration,
                    IssuerName = iName,
                    IssuerId = iId
                });
            }
        }
        public override void OnPlayerKicked(PlayerKickedEventArgs ev) {
            // Send info to server API
            if (ev.Player == null) return;
            string iName = !string.IsNullOrEmpty(ev.Issuer.Nickname) ? ev.Issuer.Nickname : "Server";
            string iId = (ev.Issuer is Player issuerPlayer) ? issuerPlayer.UserId : "Server";

            DiscordBridge.SendEvent("PlayerKicked", true, new {
                PlayerName = ev.Player.Nickname,
                PlayerId = ev.Player.UserId,
                Reasoning = ev.Reason,
                IssuerName = iName,
                IssuerId = iId
            });
        }
        public override void OnServerBanIssued(BanIssuedEventArgs ev) {
            // Send info to server API
            if (ev.BanType == BanHandler.BanType.IP) {
                DiscordBridge.SendEvent("IPBanned", true, new {
                    PlayerName = ev.BanDetails.OriginalName,
                    PlayerIp = ev.BanDetails.Id,
                    Reasoning = ev.BanDetails.Reason,
                    ExpireDate = ev.BanDetails.Expires,
                    IssuerName = ev.BanDetails.Issuer
                });
            } else {
                DiscordBridge.SendEvent("PlayerBannedEx", true, new {
                    PlayerName = ev.BanDetails.OriginalName,
                    PlayerId = ev.BanDetails.Id,
                    Reasoning = ev.BanDetails.Reason,
                    ExpireDate = ev.BanDetails.Expires,
                    IssuerName = ev.BanDetails.Issuer,
                });
            }
        }
        public override void OnServerBanUpdated(BanUpdatedEventArgs ev) {
            // Send info to server API
            if (ev.BanType == BanHandler.BanType.IP) {
                DiscordBridge.SendEvent("IPBanUpdated", true, new {
                    PlayerName = ev.BanDetails.OriginalName,
                    PlayerIp = ev.BanDetails.Id,
                    Reasoning = ev.BanDetails.Reason,
                    ExpireDate = ev.BanDetails.Expires,
                    IssuerName = ev.BanDetails.Issuer,
                });
            } else {
                DiscordBridge.SendEvent("PlayerBanUpdated", true, new {
                    PlayerName = ev.BanDetails.OriginalName,
                    PlayerId = ev.BanDetails.Id,
                    Reasoning = ev.BanDetails.Reason,
                    ExpireDate = ev.BanDetails.Expires,
                    IssuerName = ev.BanDetails.Issuer,
                });
            }
        }
        public override void OnServerBanRevoked(BanRevokedEventArgs ev) {
            // Send info to server API
            if (ev.BanType == BanHandler.BanType.IP) {
                DiscordBridge.SendEvent("IPBanRevoked", true, new {
                    PlayerName = ev.BanDetails.OriginalName,
                    PlayerIp = ev.BanDetails.Id,
                    Reasoning = ev.BanDetails.Reason,
                    ExpireDate = ev.BanDetails.Expires,
                    IssuerName = ev.BanDetails.Issuer,
                });
            } else {
                DiscordBridge.SendEvent("PlayerBanRevoked", true, new {
                    PlayerName = ev.BanDetails.OriginalName,
                    PlayerId = ev.BanDetails.Id,
                    Reasoning = ev.BanDetails.Reason,
                    ExpireDate = ev.BanDetails.Expires,
                    IssuerName = ev.BanDetails.Issuer,
                });
            }
        }
        public override void OnPlayerMuted(PlayerMutedEventArgs ev) {
            // Send info to server API
            string iName = !string.IsNullOrEmpty(ev.Issuer.Nickname) ? ev.Issuer.Nickname : "Server";
            string iId = (ev.Issuer is Player issuerPlayer) ? issuerPlayer.UserId : "Server";

            DiscordBridge.SendEvent("PlayerMuted", true, new {
                PlayerName = ev.Player.Nickname,
                PlayerId = ev.Player.UserId,
                IsIntercom = ev.IsIntercom ? "Intercom" : "Standard",
                IssuerName = iName,
                IssuerId = iId
            });
        }
        public override void OnPlayerUnmuted(PlayerUnmutedEventArgs ev) {
            // Send info to server API
            string iName = !string.IsNullOrEmpty(ev.Issuer.Nickname) ? ev.Issuer.Nickname : "Server";
            string iId = (ev.Issuer is Player issuerPlayer) ? issuerPlayer.UserId : "Server";

            DiscordBridge.SendEvent("PlayerUnmuted", true, new {
                PlayerName = ev.Player.Nickname,
                PlayerId = ev.Player.UserId,
                IsIntercom = ev.IsIntercom ? "Intercom" : "Standard",
                IssuerName = iName,
                IssuerId = iId
            });
        }
        public override void OnPlayerReportedCheater(PlayerReportedCheaterEventArgs ev)
        {
            DiscordBridge.SendEvent("PlayerReportedCheater", true, new
            {
                ReporterName = ev.Player.Nickname,
                ReporterId = ev.Player.UserId,
                ReportedName = ev.Target.Nickname,
                ReportedId = ev.Target.UserId,
                Reasoning = ev.Reason
            });
        }
        public override void OnPlayerReportedPlayer(PlayerReportedPlayerEventArgs ev)
        {
            DiscordBridge.SendEvent("PlayerReportedPlayer", true, new
            {
                ReporterName = ev.Player.Nickname,
                ReporterId = ev.Player.UserId,
                ReportedName = ev.Target.Nickname,
                ReportedId = ev.Target.UserId,
                Reasoning = ev.Reason
            });
        }
        
        private string GetDamageType(DamageHandlerBase handlerBase)
        {
            switch (handlerBase)
            {
                case Scp049DamageHandler:
                    return "SCP-049";
                case Scp939DamageHandler:
                    return "SCP-939";
                case Scp096DamageHandler:
                    return "SCP-096";
                case Scp3114DamageHandler:
                    return "SCP-3114";
                case Scp018DamageHandler:
                    return "SCP-018";
                case Scp1507DamageHandler:
                    return "SCP-1507";
                case Scp956DamageHandler:
                    return "SCP-956";
                case ScpDamageHandler:
                    return "SCP";
                // Weapons
                case CustomReasonFirearmDamageHandler customHandler:
                    return customHandler.WeaponType.ToString();
                case FirearmDamageHandler firearmHandler:
                    return firearmHandler.WeaponType.ToString();
                case DisruptorDamageHandler:
                    return "X-3 Disruptor";
                case JailbirdDamageHandler:
                    return "JailBird";
                case MicroHidDamageHandler:
                    return "Micro-HID";
                case Scp1509DamageHandler:
                    return "SCP-1509";
                // Others
                case ExplosionDamageHandler:
                    return "Explosion";
                case RecontainmentDamageHandler:
                    return "Recontained";
                case WarheadDamageHandler:
                    return "Alpha Warhead";
                case UniversalDamageHandler universal:
                    return DeathTranslations.TranslationsById.TryGetValue(universal.TranslationId, out DeathTranslation translation) ?
                        translation.LogLabel : $"Unknown ({universal.TranslationId})";

                default:
                    return "Unknown";
            }
        }
    }
}
