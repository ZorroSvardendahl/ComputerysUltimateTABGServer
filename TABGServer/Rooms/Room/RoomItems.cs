﻿using TABGCommunityServer.MiscDataTypes;

namespace TABGCommunityServer.Rooms
{
    public partial class Room
    {
        public Dictionary<int, Item> Items { get; private set; } = new Dictionary<int, Item>();
        public int CurrentID = 0;

        public void SpawnItem(Item item)
        {
            Items[item.Id] = item;
            CurrentID++;
        }

        public void RemoveItem(Item item)
        {
            Items.Remove(item.Id);
        }

        public static byte[] ClientRequestDrop(BinaryReader binaryReader, Room room)
        {
            // player
            var playerIndex = binaryReader.ReadByte();
            // item id
            var itemID = binaryReader.ReadInt32();
            // count of items
            var itemCount = binaryReader.ReadInt32();

            // location
            var x = binaryReader.ReadSingle();
            var y = binaryReader.ReadSingle();
            var z = binaryReader.ReadSingle();

            int networkID = room.CurrentID;
            Item weapon = new Item(networkID, itemID, itemCount, (x, y, z));
            room.SpawnItem(weapon);

            return SendItemDropPacket(networkID, itemID, itemCount, (x, y, z));
        }

        public static byte[] SendItemDropPacket(int index, int type, int count, (float x, float y, float z) loc)
        {
            byte[] sendByte = new byte[512];
            using (MemoryStream writerMemoryStream = new MemoryStream(sendByte))
            {
                using (BinaryWriter binaryWriterStream = new BinaryWriter(writerMemoryStream))
                {
                    // index (doesn't seem to serve a purpose)
                    binaryWriterStream.Write((Int32)index);
                    // type
                    binaryWriterStream.Write(type);
                    // quantity
                    binaryWriterStream.Write(count);

                    // location
                    binaryWriterStream.Write(loc.x);
                    binaryWriterStream.Write(loc.y);
                    binaryWriterStream.Write(loc.z);
                }
            }
            return sendByte;
        }

        public static byte[] ClientRequestPickUp(BinaryReader binaryReader, Room room)
        {
            // player
            var playerIndex = binaryReader.ReadByte();
            // item network index
            var netIndex = binaryReader.ReadInt32();
            // item slot of player
            var itemSlot = binaryReader.ReadByte();

            Item weapon = room.Items[netIndex];

            // clean up DB
            room.RemoveItem(weapon);

            return SendWeaponPickUpAcceptedPacket(playerIndex, netIndex, weapon.Type, weapon.Count, itemSlot);
        }

        public static byte[] SendWeaponPickUpAcceptedPacket(byte playerID, int networkIndex, int weaponID, int quantity, byte slot)
        {
            byte[] sendByte = new byte[512];
            using (MemoryStream writerMemoryStream = new MemoryStream(sendByte))
            {
                using (BinaryWriter binaryWriterStream = new BinaryWriter(writerMemoryStream))
                {
                    // player id
                    binaryWriterStream.Write(playerID);
                    // network index of the gun
                    binaryWriterStream.Write(networkIndex);
                    // weapon index in the game (id)
                    binaryWriterStream.Write(weaponID);
                    // quantity
                    binaryWriterStream.Write(quantity);
                    // slot
                    binaryWriterStream.Write(slot);
                }
            }
            return sendByte;
        }

        public static byte[] ClientRequestThrow(BinaryReader binaryReader)
        {
            // player
            var playerIndex = binaryReader.ReadByte();
            // not sure what this is..? maybe the throwable ID?
            var throwableID = binaryReader.ReadInt32();
            // count of throwables
            var throwableCount = binaryReader.ReadInt32();

            // location
            var x = binaryReader.ReadSingle();
            var y = binaryReader.ReadSingle();
            var z = binaryReader.ReadSingle();

            // rotation
            var rotX = binaryReader.ReadSingle();
            var rotY = binaryReader.ReadSingle();
            var rotZ = binaryReader.ReadSingle();

            return SendItemThrowPacket(playerIndex, throwableID, throwableCount, (x, y, z), (rotX, rotY, rotZ));
        }

        public static byte[] SendItemThrowPacket(byte thrower, int throwable, int count, (float x, float y, float z) loc, (float rotX, float rotY, float rotZ) rot)
        {
            byte[] sendByte = new byte[512];
            using (MemoryStream writerMemoryStream = new MemoryStream(sendByte))
            {
                using (BinaryWriter binaryWriterStream = new BinaryWriter(writerMemoryStream))
                {
                    // thrower
                    binaryWriterStream.Write(thrower);
                    // network index (?)
                    binaryWriterStream.Write((Int32)0);
                    // throwable id
                    binaryWriterStream.Write(throwable);
                    // count of throwables
                    binaryWriterStream.Write(count);

                    // location
                    binaryWriterStream.Write(loc.x);
                    binaryWriterStream.Write(loc.y);
                    binaryWriterStream.Write(loc.z);

                    // rotation
                    binaryWriterStream.Write(rot.rotX);
                    binaryWriterStream.Write(rot.rotY);
                    binaryWriterStream.Write(rot.rotZ);

                    // projectileSyncWatcher (not sure what this is, but it will Throw if it's true)
                    binaryWriterStream.Write(false);
                }
            }
            return sendByte;
        }
    }
}
