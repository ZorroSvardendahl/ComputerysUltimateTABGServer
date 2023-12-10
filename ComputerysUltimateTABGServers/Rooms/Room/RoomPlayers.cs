﻿using ComputerysUltimateTABGServer.DataTypes.Player;
using ENet;
using System.Text;

namespace ComputerysUltimateTABGServer.Rooms
{
    public partial class Room
    {
        public Dictionary<byte, Player> m_Players { get; private set; } = [];
        public Dictionary<byte, Group> m_Groups { get; private set; } = [];

        public int playerLives;
        public ushort killsToWin;

        public void AddPlayer(Player player)
        {
            m_Players[player.m_PlayerID] = player;
            m_Groups[player.m_GroupIndex].m_PlayerIDs.Add(player.m_PlayerID);
        }

        public void RemovePlayer(Player player)
        {
            m_Players.Remove(player.m_PlayerID);
        }

        public bool TryToGetPlayer(Peer peer, out Player? player)
        {
            if (m_Players.TryGetValue((byte)peer.ID, out player)) {
                return true;
            }
            return false;
        }
        public bool TryToGetPlayer(byte peerID, out Player? player)
        {
            if (m_Players.TryGetValue(peerID, out player))
            {
                return true;
            }
            return false;
        }

        public byte FindOrCreateGroup(ulong loginKey, bool autoTeam)
        {
            Group? groupWithLoginKey = m_Groups.Values.FirstOrDefault(g => g.m_GroupLoginKey == loginKey);
            if (groupWithLoginKey != null)
            {
                return groupWithLoginKey.m_GroupIndex;
            }

            if (autoTeam)
            {
                Group? autoFillGroup = m_Groups.Values.FirstOrDefault(g => g.m_ShouldAutoFill && g.m_PlayerIDs.Count() < g.m_MaxPlayers);
                if (autoFillGroup != null)
                {
                    return autoFillGroup.m_GroupIndex;
                }
            }

            byte groupIndex = m_Groups.Keys.Count == 0 ? (byte)0 : (byte)(m_Groups.Keys.Max() + 1);
            m_Groups.Add(groupIndex, new Group(autoTeam, loginKey, groupIndex));
            return groupIndex;
        }
    }
}