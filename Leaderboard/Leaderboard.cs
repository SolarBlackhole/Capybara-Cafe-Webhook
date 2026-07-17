using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Hints;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079.Map;

namespace CapybaraCafePlugin.Leaderboard
{
    public class ScpPlayerData
    {
        public int Kills { get; set;}
        public RoleTypeId RoleTypeId_ { get; set;}
        public void IncrementKills()
        {
            Kills++;
        }
    }
    public class CurrentLeaderboard
    {
        private Player fastestEscapePlayer = null;
        private TimeSpan escapeTime;
        private RoleTypeId escapeRole;
        private Dictionary<Player, ScpPlayerData> ScpTeam = new Dictionary<Player, ScpPlayerData>();

        public void SetFastestEscape(Player player, TimeSpan time, RoleTypeId role)
        {
            fastestEscapePlayer = player;
            escapeTime = time;
            escapeRole = role;
        }
        public void ResetFastestEscape()
        {
            fastestEscapePlayer = null;
        }
        public bool AnyEscapes()
        {
            return fastestEscapePlayer != null ? true : false;
        }
        public string GetFastestEscapeMessage()
        {
            if (!AnyEscapes())
            {
                return null;
            }
            string baseText = "<color=#22EC22>" + fastestEscapePlayer.Nickname + "</color> escaped in " + escapeTime.ToString(@"mm\:ss") + " as a ";
            if (escapeRole == RoleTypeId.ClassD)
            {
                return baseText + "<color=#ff8e00>Class-D</color>";
            } 
            else if (escapeRole == RoleTypeId.Scientist)
            {
                return baseText + "<color=#ffff7c>Scientist</color>";
            }
            return null;
        }
        public void AddSCPKill(Player player)
        {
            if (ScpTeam.ContainsKey(player))
            {
                ScpTeam[player].IncrementKills();
            } 
            else
            {
                ScpTeam.Add(player, new ScpPlayerData { Kills = 1, RoleTypeId_ = player.Role });
            }
        }
        public string GetSCPMostKills()
        {
            if (ScpTeam.Count == 0)
            {
                return null;
            }
            else
            {
                int maxKills = 0;
                Player topKiller = null;
                RoleTypeId topKillerRole = RoleTypeId.None;
                foreach (var ScpMember in ScpTeam)
                {
                    if (ScpMember.Value.Kills > maxKills)
                    {
                        maxKills = ScpMember.Value.Kills;
                        topKiller = ScpMember.Key;
                        topKillerRole = ScpMember.Value.RoleTypeId_;
                    }
                }
                string baseText = "";
                if (maxKills == 1)
                {
                    baseText = "<color=#EC2222>" + topKiller.Nickname + "</color> killed " + maxKills + " player as " + ScpStringFormater(topKillerRole);
                }
                else
                {
                    baseText = "<color=#EC2222>" + topKiller.Nickname + "</color> killed " + maxKills + " players as " + ScpStringFormater(topKillerRole);
                }
                return topKiller != null ? baseText : null;
            }
        }
        private string ScpStringFormater(RoleTypeId role)
        {
            switch (role)
            {
                case RoleTypeId.Scp049:
                    return "<color=#EC2222>SCP-049</color>";
                case RoleTypeId.Scp096:
                    return "<color=#EC2222>SCP-096</color>";
                case RoleTypeId.Scp173:
                    return "<color=#EC2222>SCP-173</color>";
                case RoleTypeId.Scp939:
                    return "<color=#EC2222>SCP-939</color>";
                case RoleTypeId.Scp106:
                    return "<color=#EC2222>SCP-106</color>";
                case RoleTypeId.Scp079:
                    return "<color=#EC2222>SCP-079</color>";
                case RoleTypeId.Scp0492:
                    return "<color=#EC2222>SCP-049-2</color>";
                case RoleTypeId.Scp3114:
                    return "<color=#EC2222>SCP-3114</color>";
                case RoleTypeId.Flamingo:
                case RoleTypeId.AlphaFlamingo:
                case RoleTypeId.ZombieFlamingo:
                    return "<color=#ff58ca>Flamingo</color>";
                default:
                    return null;
            }
        }
    }
}