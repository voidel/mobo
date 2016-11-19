

using System;
/**
* Author: Christopher Cola
* Created on 11/11/2015
*/
using Lidgren.Network;
using System.Xml;
using System.Collections.Generic;
using System.Linq;

namespace MoboServerWPF
{
    class Network
    {
        // The server itself
        public static NetServer Server;

        // The server configuration
        public static NetPeerConfiguration Config;

        // The messages that come from the Mobo clients
        static NetIncomingMessage in_message;

        // The messages that are sent to the Mobo clients
        public static NetOutgoingMessage out_message;

        // Indentifying id of host
        public static long host_uid = -1;

        // Message types
        public const byte CONNECT = 100;
        public const byte MOVE = 101;
        public const byte DISCONNECT = 102;

        public const byte CREATE_PROJECTILE = 150;
        public const byte REMOVE_PROJECTILE = 151;
        public const byte CREATE_STATION_PROJECTILE = 152;

        public const byte CREATE_STATION = 160;
        public const byte REMOVE_STATION = 161;
        public const byte REMOVE_NODE = 162;

        public const byte HEALTH = 200;
        public const byte KILL_PLAYER = 201;
        public const byte SCORE = 202;

        public const byte NEW_HOST = 210;

        public static void Update()
        {
            // Read messages if they are not null and act according to the 'header' string
            while((in_message = Server.ReadMessage()) != null)
            {
                if (in_message.MessageType == NetIncomingMessageType.Data)
                {
                    switch (in_message.ReadByte())
                    {
                        case CONNECT: Connect(); break;
                        case MOVE: Move(); break;
                        case DISCONNECT: Disconnect(); break;
                        case CREATE_PROJECTILE: CreateProjectile(); break;
                        case REMOVE_PROJECTILE: RemoveProjectile(); break;
                        case CREATE_STATION_PROJECTILE: CreateStationProjectile(); break;
                        case CREATE_STATION: CreateStation(); break;
                        case REMOVE_STATION: RemoveStation(); break;
                        case REMOVE_NODE: RemoveNode(); break;
                        case HEALTH: Health(); break;
                        case SCORE: Score(); break;
                    }
                }
            }
        }

        // Mobo client is connecting to the server
        public static void Connect()
        {
            // Read client indentifier
            long uid = in_message.ReadInt64();

            // Read name of player
            string playername = in_message.ReadString();

            // Read position of player
            int x = in_message.ReadInt32();
            int y = in_message.ReadInt32();

            // Wait a little to make sure client is connected
            System.Threading.Thread.Sleep(100);

            // Create new player object for connencting client
            Player playerNew = new Player(playername, x, y, 0, uid);
            Player.players.TryAdd(playerNew.uid, playerNew);

            // Now all the current players in the server to the clients
            foreach (Player player in Player.players.Values)
            {
                // Write a new message with incoming parameters, and send the all connected clients.
                out_message = Server.CreateMessage();
                out_message.Write(CONNECT);
                out_message.Write(player.name);
                out_message.Write(player.uid);
                out_message.Write(player.x);
                out_message.Write(player.y);
                out_message.Write(player.health);
                out_message.Write(player.score);

                // Send via TCP to make sure the new player recieves every one
                Server.SendMessage(out_message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
            }

            Program.window.Append(string.Format("{0} (UID:{1}) connected.",playername,uid));

            // Now send all stations
            foreach (KeyValuePair<int, XmlDocument> entry in Stations.stations)
            {
                out_message = Server.CreateMessage();
                out_message.Write(CREATE_STATION);
                out_message.Write(entry.Key);
                out_message.Write(entry.Value.OuterXml);

                // Send via TCP to make sure the new player recieves every one
                Server.SendMessage(out_message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        // Recieve positions from a client and store them
        public static void Move()
        {
            long uid = in_message.ReadInt64();
            int x = in_message.ReadInt32();
            int y = in_message.ReadInt32();
            float rot = in_message.ReadFloat();

            if(Player.players.ContainsKey(uid))
            {
                Player toFindPlayer = Player.players[uid];

                toFindPlayer.x = x;
                toFindPlayer.y = y;
                toFindPlayer.rotation = rot;
                toFindPlayer.timeout = 0;
            }
        }

        // Mobo client is disconnecting from the server
        public static void Disconnect()
        {
            long uid = in_message.ReadInt64();

            // Use a lamda expression to find the player and connection with this uid
            Player toFindPlayer = Player.players[uid];
            NetConnection toFindConnection = Server.Connections.Find(item => item.RemoteUniqueIdentifier == uid);

            toFindConnection.Disconnect("bye");

            // Allow some time to disconnect
            System.Threading.Thread.Sleep(100);

            if (Server.ConnectionsCount != 0)
            {
                out_message = Server.CreateMessage();
                out_message.Write(DISCONNECT);
                out_message.Write(uid);

                Server.SendMessage(out_message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
            }

            Player.players.Remove(toFindPlayer.uid);
            Program.window.Append(toFindPlayer.name + " left. (disconnect by user)");
        }

        // A player/client has fired a bullet
        private static void CreateProjectile()
        {
            // Owner of the projectile
            long uid = in_message.ReadInt64();

            // Unique index of the projectile
            int index = in_message.ReadInt32();

            // Starting position of projectile
            int x = in_message.ReadInt32();
            int y = in_message.ReadInt32();

            // Starting rotation of projectile
            float rot = in_message.ReadFloat();

            // Starting velocity of a projectile
            float velocity_x = in_message.ReadFloat();
            float velocity_y = in_message.ReadFloat();

            // Now send it to all clients
            out_message = Server.CreateMessage();
            out_message.Write(CREATE_PROJECTILE);
            out_message.Write(uid);
            out_message.Write(index);
            out_message.Write(x);
            out_message.Write(y);
            out_message.Write(rot);
            out_message.Write(velocity_x);
            out_message.Write(velocity_y);

            // Send via TCP to make sure all players recieve it
            Server.SendMessage(out_message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        // A projectile needs to be removed from all clients (it hit something)
        private static void RemoveProjectile()
        {
            // Owner of the projectile
            long uid = in_message.ReadInt64();

            // Unique index of the projectile
            int index = in_message.ReadInt32();

            // Now send it to all clients
            out_message = Server.CreateMessage();
            out_message.Write(REMOVE_PROJECTILE);
            out_message.Write(uid);
            out_message.Write(index);

            // Send via TCP to make sure all players recieve it
            Server.SendMessage(out_message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        // A stationNode has fired a bullet
        private static void CreateStationProjectile()
        {
            // Client of the projectile
            long uid = in_message.ReadInt64();

            // Id of the owner Station
            int stationId = in_message.ReadInt32();

            // Id of the owner Node
            int nodeId = in_message.ReadInt32();

            // Starting position of projectile
            int x = in_message.ReadInt32();
            int y = in_message.ReadInt32();

            // Starting velocity of a projectile
            float velocity_x = in_message.ReadFloat();
            float velocity_y = in_message.ReadFloat();

            // Now send it to all clients
            out_message = Server.CreateMessage();
            out_message.Write(CREATE_STATION_PROJECTILE);
            out_message.Write(uid);
            out_message.Write(stationId);
            out_message.Write(nodeId);
            out_message.Write(x);
            out_message.Write(y);
            out_message.Write(velocity_x);
            out_message.Write(velocity_y);

            // Send via TCP to make sure all players recieve it
            Server.SendMessage(out_message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private static void CreateStation()
        {
            // Creator of the station
            long uid = in_message.ReadInt64();

            // Unique id of the station
            int index = in_message.ReadInt32();

            // XML representation of the station
            string stationXML = in_message.ReadString();

            // Add station to list if creator uid is the host
            if(uid == host_uid)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(stationXML);
                Stations.stations.TryAdd(index, doc);
                Program.window.Append("New Station " + index + " has appeared!");
            }

            // Now send all stations
            foreach (KeyValuePair<int, XmlDocument> entry in Stations.stations)
            {
                out_message = Server.CreateMessage();
                out_message.Write(CREATE_STATION);
                out_message.Write(entry.Key);
                out_message.Write(entry.Value.OuterXml);

                // Send via TCP to make sure the new player recieves every one
                Server.SendMessage(out_message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        private static void RemoveStation()
        {
            // Unique id of the station
            int stationId = in_message.ReadInt32();

            Stations.stations.Remove(stationId);

            Program.window.Append("Station " + stationId + " was completely destroyed!");

            // Now send to all clients
            out_message = Server.CreateMessage();
            out_message.Write(REMOVE_STATION);
            out_message.Write(stationId);
            Server.SendMessage(out_message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private static void RemoveNode()
        {
            // Unique id of the station
            int stationId = in_message.ReadInt32();

            // Unique id of the node
            int nodeId = in_message.ReadInt32();

            Stations.RemoveNode(stationId, nodeId);

            // Now send to all clients
            out_message = Server.CreateMessage();
            out_message.Write(REMOVE_NODE);
            out_message.Write(stationId);
            out_message.Write(nodeId);
            Server.SendMessage(out_message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        // A player requires an update to health
        private static void Health()
        {
            // Player the health change relates to
            long uid = in_message.ReadInt64();

            // The difference in health
            int healthChange = in_message.ReadInt32();

            // Use a lamda expression to find the player and connection with this uid
            Player toFindPlayer = Player.players[uid];

            toFindPlayer.health += healthChange;

            // Is the player dead?
            if (toFindPlayer.health <= 0)
            {
                out_message.Write(KILL_PLAYER);
                out_message.Write(toFindPlayer.uid);
                Server.SendMessage(out_message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                Program.window.Append(string.Format("{0} (UID:{1}) died.", toFindPlayer.name, toFindPlayer.uid));
                toFindPlayer.health = 100;
                toFindPlayer.x = 0;
                toFindPlayer.y = 0;
            }

            // Now send to all clients
            out_message = Server.CreateMessage();
            out_message.Write(HEALTH);
            out_message.Write(uid);
            out_message.Write(toFindPlayer.health);
            Server.SendMessage(out_message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        // A player requires an update to score
        private static void Score()
        {
            // Player the score change relates to
            long uid = in_message.ReadInt64();

            // The difference in score
            int scoreChange = in_message.ReadInt32();

            // Use a lamda expression to find the player and connection with this uid
            Player toFindPlayer = Player.players[uid];

            toFindPlayer.score += scoreChange;

            // Now send to all clients
            out_message = Server.CreateMessage();
            out_message.Write(SCORE);
            out_message.Write(uid);
            out_message.Write(toFindPlayer.score);
            Server.SendMessage(out_message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }
    }
}
