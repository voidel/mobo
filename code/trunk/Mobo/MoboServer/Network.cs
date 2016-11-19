/**
* Author: Christopher Cola
* Created on 11/11/2015
*/

using Lidgren.Network;

namespace MoboServer
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

        // Message types
        public const byte CONNECT = 100;
        public const byte MOVE = 101;
        public const byte DISCONNECT = 102;

        public const byte CREATE_PROJECTILE = 150;
        public const byte REMOVE_PROJECTILE = 151;

        public const byte HEALTH = 200;

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
                        case HEALTH: Health(); break;
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
            Player.players.Add(new Player(playername, x, y, 0, uid));

            // Now send this new player all the players in the server
            foreach (Player player in Player.players)
            {
                // Write a new message with incoming parameters, and send the all connected clients.
                out_message = Server.CreateMessage();
                out_message.Write(CONNECT);
                out_message.Write(player.name);
                out_message.Write(player.uid);
                out_message.Write(player.x);
                out_message.Write(player.y);

                // Send via TCP to make sure the new player recieves every one
                Server.SendMessage(out_message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
            }

            Program.log.Add(string.Format("{0} (UID:{1}) connected.",playername,uid));
        }

        // Recieve positions from a client and store them
        public static void Move()
        {
            long uid = in_message.ReadInt64();
            int x = in_message.ReadInt32();
            int y = in_message.ReadInt32();
            float rot = in_message.ReadFloat();

            foreach (Player player in Player.players)
            {
                if(player.uid == uid)
                {
                    player.x = x;
                    player.y = y;
                    player.rotation = rot;
                    player.timeout = 0;
                }
            }
        }

        // Mobo client is disconnecting from the server
        public static void Disconnect()
        {
            long uid = in_message.ReadInt64();

            for (int i = 0; i < Player.players.Count; i++)
            {
                if (Player.players[i].uid == uid)
                {
                    // Disconnect the player
                    Server.Connections[i].Disconnect("bye");

                    Program.log.Add(Player.players[i].name + " left. (disconnect by user)");

                    // Allow some time to disconnect
                    System.Threading.Thread.Sleep(100);

                    if (Server.ConnectionsCount != 0)
                    {
                        out_message = Server.CreateMessage();
                        out_message.Write(DISCONNECT);
                        out_message.Write(Player.players[i].uid);

                        Server.SendMessage(out_message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                    }

                    Player.players.RemoveAt(i);
                    i--;
                    break;
                }
            }
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

        // A player requires an update to health
        private static void Health()
        {
            // Player the health change relates to
            long uid = in_message.ReadInt64();

            // The difference in health
            int healthChange = in_message.ReadInt32();

            // Now send to all clients
            out_message = Server.CreateMessage();
            out_message.Write(HEALTH);
            out_message.Write(uid);
            out_message.Write(healthChange);
            Server.SendMessage(out_message, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }
    }
}
