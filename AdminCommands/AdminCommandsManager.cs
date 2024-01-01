﻿using ComputerysUltimateTABGServer.DataTypes.Player;
using ComputerysUltimateTABGServer.Rooms;
using ENet;
using System.Collections.Frozen;

namespace ComputerysUltimateTABGServer.AdminCommands
{
    // This entire thing feels like a hack
    public static partial class AdminCommandManager
    {
        // I'm not sure if i like this, but it works for now
        public static readonly FrozenDictionary<string, AdminCommandDelegate> UserCommands = new Dictionary<string, AdminCommandDelegate>
        {

        }.ToFrozenDictionary();
        public static readonly FrozenDictionary<string, AdminCommandDelegate> ModeratorCommands = new Dictionary<string, AdminCommandDelegate>
        {

        }.ToFrozenDictionary();
        public static readonly FrozenDictionary<string, AdminCommandDelegate> AdminCommands = new Dictionary<string, AdminCommandDelegate>
        {
        }.ToFrozenDictionary();
        public static readonly FrozenDictionary<string, AdminCommandDelegate> OwnerCommands = new Dictionary<string, AdminCommandDelegate>
        {

        }.ToFrozenDictionary();

        public static void HandleAdminCommand(Peer peer, byte[] receivedPacketRaw, Room room)
        {
            byte PlayerID = receivedPacketRaw[0];
            byte MessageLength = receivedPacketRaw[1];
            string Message = System.Text.Encoding.UTF8.GetString(receivedPacketRaw, 2, MessageLength);
            if (Message[0] != '/') { return; }

            Message = Message[1..];
            string[] CommandParts = Message.Split(' ');
            string Command = CommandParts[0];
            CommandParts = CommandParts[1..];
            CommandParts = CommandParts.Select(item => item.ToLower()).ToArray();

            if (!room.m_Players.TryGetValue(PlayerID, out Player? commandSender)) { return; }

            commandSender.m_PermissionLevel = PermissionLevel.Admin; // TODO: Remove this

            if (commandSender.m_PermissionLevel >= PermissionLevel.Owner)
            {
                if (OwnerCommands.TryGetValue(Command, out AdminCommandDelegate? CommandHandler))
                {
                    CommandHandler(peer, CommandParts, room); return;
                }
            }
            if (commandSender.m_PermissionLevel >= PermissionLevel.Admin)
            {
                if (AdminCommands.TryGetValue(Command, out AdminCommandDelegate? CommandHandler))
                {
                    CommandHandler(peer, CommandParts, room); return;
                }
            }
            if (commandSender.m_PermissionLevel >= PermissionLevel.Moderator)
            {
                if (ModeratorCommands.TryGetValue(Command, out AdminCommandDelegate? CommandHandler))
                {
                    CommandHandler(peer, CommandParts, room); return;
                }
            }
            if (UserCommands.TryGetValue(Command, out AdminCommandDelegate? UserCommandHandler)) { UserCommandHandler(peer, CommandParts, room); }
        }

        public delegate void AdminCommandDelegate(Peer peer, string[] CommandParts, Room room);
    }

    public enum PermissionLevel
    {
        User = 0,
        Moderator = 1,
        Admin = 2,
        Owner = 3
    }
}