/**
* Author: Christopher Cola
* Created on 14/11/2015
*/

using System;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace Mobo
{
    // The class that establishes the connection to the server and reads incoming messages if online
    class Network
    {
        // The client itself
        public static NetClient Client;

        // The server configuration
        public static NetPeerConfiguration Config;

        // The messages that come from the Mobo clients
        static NetIncomingMessage in_message;

        // The messages that are sent to the Mobo clients
        public static NetOutgoingMessage out_message;

        // Are we connected?
        public static bool connected = false;

        // Are we the host?
        public static bool host = false;

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

        internal static void Initialize()
        {
            // Clean out the player list
            Online.players.Clear();

            // Start Client and attempt connection to the MoboServer
            Config = new NetPeerConfiguration("Mobo");
            Client = new NetClient(Network.Config);
            Client.Start();
            Client.Connect(SettingsManager.getServerIP(), SettingsManager.getPort());
            Console.WriteLine(Client.UniqueIdentifier);

            System.Threading.Thread.Sleep(300);

            // Send a connection request
            out_message = Client.CreateMessage();
            out_message.Write(CONNECT);
            out_message.Write(Client.UniqueIdentifier);
            out_message.Write(SettingsManager.getUsername());
            out_message.Write(0);
            out_message.Write(0);
            Client.SendMessage(out_message, NetDeliveryMethod.ReliableOrdered);

            // Wait a little to ensure message sent
            System.Threading.Thread.Sleep(300);

            // Use client status to work out whether we are connected or not
            string status = Client.ConnectionStatus.ToString();
            if (status.Equals("None"))
            {
                status = "Server ONLINE";
                connected = true;
            }
            if (status.Equals("Disconnected"))
            {
                status = "Server OFFLINE";
                connected = false;
            }

            ScreenManager.messageList.Add(Client.Status.ToString(), MessageType.Network);
            ScreenManager.messageList.Add(status, MessageType.Network);
        }

        public static void Update()
        {
            // Read messages if they are not null and act according to the 'header' string
            while ((in_message = Client.ReadMessage()) != null)
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
                        case REMOVE_NODE: RemoveNode(); break;
                        case CREATE_STATION: CreateStation(); break;
                        case REMOVE_STATION: RemoveStation(); break;
                        case HEALTH: Health(); break;
                        case KILL_PLAYER: KillPlayer(); break;
                        case SCORE: Score(); break;
                        case NEW_HOST: NewHost(); break;
                    }
                    Client.Recycle(in_message);
                }
            }
        }

        private static void Connect()
        {
            // Whenever a new player connects, details about that player, including
            // details about all current connected players are sent to all clients
            // This ensures the new player has knowledge of all currently connected
            // players and existing players know about the new player

            string name = in_message.ReadString();
            long uid = in_message.ReadInt64();
            int x = in_message.ReadInt32();
            int y = in_message.ReadInt32();
            int health = in_message.ReadInt32();
            int score = in_message.ReadInt32();

            // We now need to add the recieved player to the list, but ONLY if that
            // player isn't already in the list

            //if toFind is null, we don't have that player yet and we need to add it
            if(!Online.players.ContainsKey(uid))
            {
                Player p = new Player(Vector2.Zero, name, health, false, uid);
                p.score = score;
                Online.players.TryAdd(p.uid, p);
                ScreenManager.messageList.Add(name + " joined the game", MessageType.Network);
            }
        }

        private static void Move()
        {
            long uid = in_message.ReadInt64();
            int x = in_message.ReadInt32();
            int y = in_message.ReadInt32();
            float rot = in_message.ReadFloat();

            if(Online.players.ContainsKey(uid))
            {
                Player toFind = Online.players[uid];

                // Don't update the position if it's the local player!
                if (!toFind.playerControlled)
                {
                    // Set the new coordinates
                    toFind.setPosition(x, y);
                    toFind.playerRotation = rot;
                }
            }
        }

        private static void Disconnect()
        {
            long uid = in_message.ReadInt64();

            // Only disconnect me if the disconnected uid is my uid
            if (uid == Client.UniqueIdentifier)
            {
                ScreenManager.messageList.Add("Kicked (timeout)", MessageType.Network);
                connected = false;
            }
            else
            {
                if (Online.players.ContainsKey(uid))
                {
                    Player toFind = Online.players[uid];

                    ScreenManager.messageList.Add(toFind.name + " left.", MessageType.Network);

                    // Remove player
                    Online.players.Remove(uid);
                }
            }
        }

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

            // Starting velocity of projectile
            float velocity_x = in_message.ReadFloat();
            float velocity_y = in_message.ReadFloat();
            Vector2 velocity = new Vector2(velocity_x, velocity_y);

            if (Online.players.ContainsKey(uid))
            {
                Player toFind = Online.players[uid];

                // Don't do anything if the projectile was fired by the local player
                if (!toFind.playerControlled)
                {
                    Projectile newProjectile = new Projectile(ContentStore.laser_blue, new Vector2(x, y), velocity, rot, index, toFind, null);
                    toFind.projectiles.TryAdd(newProjectile.index, newProjectile);
                }
            }
        }

        private static void RemoveProjectile()
        {
            // Owner of the projectile
            long uid = in_message.ReadInt64();

            // Unqiue index of the projectile
            int index = in_message.ReadInt32();

            if (Online.players.ContainsKey(uid))
            {
                Player toFindPlayer = Online.players[uid];

                if(toFindPlayer.projectiles.ContainsKey(index))
                {
                    Projectile toFindProjectile = toFindPlayer.projectiles[index];

                    // Do the removal
                    if (toFindProjectile != null)
                    {
                        toFindProjectile.DeathAnim();
                    }
                }
            }
        }

        // A stationNode has fired a bullet
        private static void CreateStationProjectile()
        {
            // Client of the projectile
            long uid = in_message.ReadInt64();

            // Unqiue index of the station
            int stationId = in_message.ReadInt32();

            // NodeId of the owner node
            int nodeId = in_message.ReadInt32();

            // Starting position of projectile
            int x = in_message.ReadInt32();
            int y = in_message.ReadInt32();

            // Starting velocity of a projectile
            float velocity_x = in_message.ReadFloat();
            float velocity_y = in_message.ReadFloat();

            // We don't duplicate the projectile if it's already there
            if(uid != Client.UniqueIdentifier)
            {
                if (Online.spawner.stations.ContainsKey(stationId))
                {
                    Station toFindStation = Online.spawner.stations[stationId];

                    if (toFindStation.flat_list.ContainsKey(nodeId))
                    {
                        Node toFindNode = toFindStation.flat_list[nodeId];

                        Vector2 position = new Vector2(x, y);
                        Vector2 velocity = new Vector2(velocity_x, velocity_y);

                        Projectile newProjectile = new Projectile(ContentStore.laser_yellow, position, velocity, 0f, -1, null, toFindNode.data);
                        toFindNode.data.projectiles.TryAdd(newProjectile.index, newProjectile);
                    }
                }
            }
        }

        private static void CreateStation()
        {
            // Unqiue index of the station
            int stationId = in_message.ReadInt32();

            // XML representation of station
            string stationXML = in_message.ReadString();

            if(!Online.spawner.stations.ContainsKey(stationId))
            {
                Station newStation = new StationFromXML().FromXML(stationXML);
                Online.spawner.stations.TryAdd(newStation.id, newStation);
                ScreenManager.messageList.Add("A new station has appeared!", MessageType.GameHint);
                Online.spawner.availableSpot[Online.spawner.spawnPositions.IndexOf(newStation.position)] = true;
            }
        }

        private static void RemoveStation()
        {
            // Unqiue index of the station
            int stationId = in_message.ReadInt32();

            if (Online.spawner.stations.ContainsKey(stationId))
            {
                Station toFindStation = Online.spawner.stations[stationId];

                toFindStation.spawner.Destroy(toFindStation);
            }
        }

        private static void RemoveNode()
        {
            // Unqiue index of the station
            int stationId = in_message.ReadInt32();

            // Unqiue index of the node
            int nodeId = in_message.ReadInt32();

            if(Online.spawner.stations.ContainsKey(stationId))
            {
                Station toFindStation = Online.spawner.stations[stationId];

                if (toFindStation.flat_list.ContainsKey(nodeId))
                {
                    Node toFindNode = toFindStation.flat_list[nodeId];
                    toFindNode.RemoteDestroy();
                }
            }
        }

        private static void Health()
        {
            // Player the health change relates to
            long uid = in_message.ReadInt64();

            // The updated health
            int new_health = in_message.ReadInt32();

            if (Online.players.ContainsKey(uid))
            {
                Player toFind = Online.players[uid];

                toFind.health = new_health; 
            }
        }

        private static void KillPlayer()
        {
            // Player who died
            long uid = in_message.ReadInt64();

            if (Online.players.ContainsKey(uid))
            {
                Player toFind = Online.players[uid];

                toFind.health = 100;
                toFind.position = Vector2.Zero;
                ScreenManager.messageList.Add(toFind.name + " died!", MessageType.Network);
            }
        }

        private static void Score()
        {
            // Player the health change relates to
            long uid = in_message.ReadInt64();

            // The updated health
            int new_score = in_message.ReadInt32();

            if (Online.players.ContainsKey(uid))
            {
                Player toFind = Online.players[uid];

                toFind.score = new_score;
            }
        }

        // Host switch
        private static void NewHost()
        {
            // Player the who will become host
            long uid = in_message.ReadInt64();

            if(Online.localPlayer.uid == uid)
            {
                Online.spawner.max_stations = 3;
                host = true;
                ScreenManager.messageList.Add("You are now the host", MessageType.Network);
            }
        }
    }
}
